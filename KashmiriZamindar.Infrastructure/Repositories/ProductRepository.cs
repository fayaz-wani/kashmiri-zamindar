


using System.Data;
using Microsoft.Data.SqlClient;
using KashmiriZamindar.Core.Dtos;
using KashmiriZamindar.Core.Entities;
using KashmiriZamindar.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace KashmiriZamindar.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly string _connectionString;

        public ProductRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevConnection");
        }

        public async Task<List<ProductDetailDto>> GetAllProductsAsync()
        {
            var products = new List<ProductDetailDto>();

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("usp_GetAllProducts", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                products.Add(new ProductDetailDto
                {
                    ProductGuid = reader.GetGuid(reader.GetOrdinal("ProductGuid")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Category = reader.GetString(reader.GetOrdinal("Category")),
                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                    Unit = reader.GetString(reader.GetOrdinal("Unit")),
                    Description = reader.GetString(reader.GetOrdinal("Description")),
                    ImageUrl = reader.IsDBNull(reader.GetOrdinal("ImageUrl")) ? null : reader.GetString(reader.GetOrdinal("ImageUrl")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                });
            }

            return products;
        }

        public async Task<Product?> GetByGuidAsync(Guid productGuid)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand("usp_GetProductByGuid", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProductGuid", productGuid);

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync()) return null;

            return new Product
            {
                ProductGuid = reader.GetGuid(reader.GetOrdinal("ProductGuid")),
                Name = reader["Name"].ToString(),
                Category = reader["Category"].ToString(),
                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                Unit = reader["Unit"].ToString(),
                Description = reader["Description"]?.ToString(),
                ImageUrl = reader["ImageUrl"]?.ToString(),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            };
        }

        public async Task<Guid> AddProductAsync(AddProductDto product)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand("usp_AddProduct", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Name", product.Name);
            cmd.Parameters.AddWithValue("@Category", product.Category);
            cmd.Parameters.AddWithValue("@Price", product.Price);
            cmd.Parameters.AddWithValue("@Unit", product.Unit);
            cmd.Parameters.AddWithValue("@Description", product.Description);

            await con.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();

            return Guid.Parse(result.ToString());
        }
        public async Task<ProductDetailDto> GetProductWithImagesAsync(Guid guid)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("usp_GetProductWithImages", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@ProductGuid", guid);

            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            ProductDetailDto product = null;

            // ===============================
            // 1️⃣ First result set: Product
            // ===============================
            if (await reader.ReadAsync())
            {
                product = new ProductDetailDto
                {
                    ProductGuid = reader.GetGuid(reader.GetOrdinal("ProductGuid")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Category = reader.GetString(reader.GetOrdinal("Category")),
                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                    Unit = reader.GetString(reader.GetOrdinal("Unit")),
                    Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("Description")),
                    ImageUrl = reader.IsDBNull(reader.GetOrdinal("ImageUrl"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("ImageUrl")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    StockQuantity = reader.IsDBNull(reader.GetOrdinal("StockQuantity"))
                        ? 0
                        : reader.GetInt32(reader.GetOrdinal("StockQuantity")),
                    LowStockThreshold = reader.IsDBNull(reader.GetOrdinal("LowStockThreshold"))
                        ? 0
                        : reader.GetInt32(reader.GetOrdinal("LowStockThreshold")),
                    Images = new List<ProductImageDto>(),
                    RelatedProducts = new List<RelatedProductDto>()
                };
            }

            if (product == null)
                return null;

            // ===============================
            // 2️⃣ Second result set: Images
            // ===============================
            await reader.NextResultAsync();
            while (await reader.ReadAsync())
            {
                product.Images.Add(new ProductImageDto
                {
                    ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl")),
                    IsPrimary = reader.GetBoolean(reader.GetOrdinal("IsPrimary"))
                });
            }

            // ===============================
            // 3️⃣ Third result set: Reviews Summary
            // ===============================
            await reader.NextResultAsync();
            if (await reader.ReadAsync())
            {
                product.TotalReviews = reader.IsDBNull(reader.GetOrdinal("TotalReviews"))
                    ? 0
                    : reader.GetInt32(reader.GetOrdinal("TotalReviews"));

                product.AverageRating = reader.IsDBNull(reader.GetOrdinal("AverageRating"))
                    ? 0
                    : reader.GetDouble(reader.GetOrdinal("AverageRating"));

                product.RatingDistribution = new RatingDistributionDto
                {
                    Rating5Count = reader.IsDBNull(reader.GetOrdinal("Rating5Count")) ? 0 : reader.GetInt32(reader.GetOrdinal("Rating5Count")),
                    Rating4Count = reader.IsDBNull(reader.GetOrdinal("Rating4Count")) ? 0 : reader.GetInt32(reader.GetOrdinal("Rating4Count")),
                    Rating3Count = reader.IsDBNull(reader.GetOrdinal("Rating3Count")) ? 0 : reader.GetInt32(reader.GetOrdinal("Rating3Count")),
                    Rating2Count = reader.IsDBNull(reader.GetOrdinal("Rating2Count")) ? 0 : reader.GetInt32(reader.GetOrdinal("Rating2Count")),
                    Rating1Count = reader.IsDBNull(reader.GetOrdinal("Rating1Count")) ? 0 : reader.GetInt32(reader.GetOrdinal("Rating1Count"))
                };
            }

            // ===============================
            // 4️⃣ Fourth result set: Related Products
            // ===============================
            await reader.NextResultAsync();
            while (await reader.ReadAsync())
            {
                product.RelatedProducts.Add(new RelatedProductDto
                {
                    ProductGuid = reader.GetGuid(reader.GetOrdinal("ProductGuid")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Category = reader.GetString(reader.GetOrdinal("Category")),
                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                    Unit = reader.GetString(reader.GetOrdinal("Unit")),
                    ImageUrl = reader.IsDBNull(reader.GetOrdinal("ImageUrl"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("ImageUrl"))
                });
            }

            return product;
        }


        public async Task<List<ProductReviewDto>> GetProductReviewsAsync(Guid guid, int pageNumber, int pageSize)
        {
            var reviews = new List<ProductReviewDto>();

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("usp_GetProductReviews", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@ProductGuid", guid);
            cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);

            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                reviews.Add(new ProductReviewDto
                {
                    ReviewId = reader.GetInt32(reader.GetOrdinal("ReviewId")),
                    CustomerName = reader.GetString(reader.GetOrdinal("CustomerName")),
                    Rating = reader.GetInt32(reader.GetOrdinal("Rating")),
                    ReviewTitle = reader.IsDBNull(reader.GetOrdinal("ReviewTitle")) ? null : reader.GetString(reader.GetOrdinal("ReviewTitle")),
                    ReviewText = reader.IsDBNull(reader.GetOrdinal("ReviewText")) ? null : reader.GetString(reader.GetOrdinal("ReviewText")),
                    IsVerifiedPurchase = reader.GetBoolean(reader.GetOrdinal("IsVerifiedPurchase")),
                    HelpfulCount = reader.GetInt32(reader.GetOrdinal("HelpfulCount")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                });
            }

            return reviews;
        }

        public async Task<int> AddProductReviewAsync(Guid guid, AddReviewDto dto)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("usp_AddProductReview", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@ProductGuid", guid);
            cmd.Parameters.AddWithValue("@CustomerId", dto.CustomerId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CustomerName", dto.CustomerName);
            cmd.Parameters.AddWithValue("@CustomerEmail", dto.CustomerEmail ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Rating", dto.Rating);
            cmd.Parameters.AddWithValue("@ReviewTitle", dto.ReviewTitle ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ReviewText", dto.ReviewText ?? (object)DBNull.Value);

            var outputParam = new SqlParameter("@ReviewId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(outputParam);

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            return (int)outputParam.Value;
        }

        public async Task<List<RelatedProductDto>> GetRelatedProductsAsync(Guid guid)
        {
            // This is already included in GetProductWithDetailsAsync
            // But you can create a separate endpoint if needed
            var product = await GetProductWithImagesAsync(guid);
            return product?.RelatedProducts ?? new List<RelatedProductDto>();
        }
    }
}