using API.Core.Dtos;
using Dapper;
using KashmiriZamindar.Core.Dtos;
using KashmiriZamindar.Core.Entities;
using KashmiriZamindar.Core.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace KashmiriZamindar.Infrastructure.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly string _connectionString;

        public AdminRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevConnection");
        }

        // =====================================================
        // ADMIN LOGIN
        // =====================================================
        public async Task<AdminUser?> GetAdminByEmailAsync(string email)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("usp_AdminLogin", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Email", email);

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return new AdminUser
            {
                AdminId = reader.GetInt32(reader.GetOrdinal("AdminId")),
                AdminGuid = reader.GetGuid(reader.GetOrdinal("AdminGuid")),
                Email = reader["Email"].ToString(),
                PasswordHash = reader["PasswordHash"].ToString(),
                FullName = reader["FullName"].ToString(),
                Role = reader["Role"].ToString(),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            };
        }

        // =====================================================
        // DASHBOARD
        // =====================================================
        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("usp_GetDashboardStats", con);

            cmd.CommandType = CommandType.StoredProcedure;

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return new DashboardStatsDto();

            return new DashboardStatsDto
            {
                TotalOrders = reader.GetInt32(reader.GetOrdinal("TotalOrders")),
                TotalRevenue = reader.GetDecimal(reader.GetOrdinal("TotalRevenue")),
                TotalCustomers = reader.GetInt32(reader.GetOrdinal("TotalCustomers")),
                TotalProducts = reader.GetInt32(reader.GetOrdinal("TotalProducts")),
                PendingOrders = reader.GetInt32(reader.GetOrdinal("PendingOrders")),
                CompletedOrders = reader.GetInt32(reader.GetOrdinal("CompletedOrders")),
                TodayRevenue = reader.GetDecimal(reader.GetOrdinal("TodayRevenue")),
                TodayOrders = reader.GetInt32(reader.GetOrdinal("TodayOrders"))
            };
        }

        public async Task<List<RecentOrderDto>> GetRecentOrdersAsync(int top = 10)
        {
            var orders = new List<RecentOrderDto>();

            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("usp_GetRecentOrders", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Top", top);

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                orders.Add(new RecentOrderDto
                {
                    OrderGuid = reader.GetGuid(reader.GetOrdinal("OrderGuid")),
                    CustomerName = reader["CustomerName"].ToString(),
                    CustomerEmail = reader["CustomerEmail"].ToString(),
                    TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                    OrderStatus = reader["OrderStatus"].ToString(),
                    PaymentStatus = reader["PaymentStatus"].ToString(),
                    OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate"))
                });
            }

            return orders;
        }

        public async Task<List<TopProductDto>> GetTopProductsAsync(int top = 5)
        {
            var products = new List<TopProductDto>();

            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("usp_GetTopProducts", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Top", top);

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                products.Add(new TopProductDto
                {
                    ProductGuid = reader.GetGuid(reader.GetOrdinal("ProductGuid")),
                    ProductName = reader["ProductName"].ToString(),
                    Category = reader["Category"].ToString(),
                    TotalSold = reader.GetInt32(reader.GetOrdinal("TotalSold")),
                    TotalRevenue = reader.GetDecimal(reader.GetOrdinal("TotalRevenue")),
                    ImageUrl = reader["ImageUrl"]?.ToString()
                });
            }

            return products;
        }

        public async Task<List<SalesChartDto>> GetSalesChartDataAsync(int days = 7)
        {
            var data = new List<SalesChartDto>();

            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("usp_GetSalesChartData", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Days", days);

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                data.Add(new SalesChartDto
                {
                    Date = reader["Date"].ToString(),
                    Revenue = reader.GetDecimal(reader.GetOrdinal("Revenue")),
                    Orders = reader.GetInt32(reader.GetOrdinal("Orders"))
                });
            }

            return data;
        }

        // =====================================================
        // ORDERS
        // =====================================================
        public async Task<List<OrderManagementDto>> GetAllOrdersAsync(int pageNumber, int pageSize, string status)
        {
            var orders = new List<OrderManagementDto>();

            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("usp_GetAllOrders", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);
            cmd.Parameters.AddWithValue("@Status", status ?? (object)DBNull.Value);

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                orders.Add(new OrderManagementDto
                {
                    OrderGuid = reader.GetGuid(reader.GetOrdinal("OrderGuid")),
                    OrderId = reader.GetInt32(reader.GetOrdinal("OrderId")),
                    CustomerName = reader["CustomerName"].ToString(),
                    CustomerEmail = reader["CustomerEmail"].ToString(),
                    CustomerPhone = reader["CustomerPhone"].ToString(),
                    TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                    OrderStatus = reader["OrderStatus"].ToString(),
                    PaymentStatus = reader["PaymentStatus"].ToString(),
                    PaymentType = reader["PaymentType"].ToString(),
                    OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                    ShippingAddress = reader["ShippingAddress"].ToString()
                });
            }

            return orders;
        }

        public async Task<OrderManagementDto?> GetOrderDetailsAsync(Guid orderGuid)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("usp_GetOrderDetails", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@OrderGuid", orderGuid);

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            OrderManagementDto order = null;

            if (await reader.ReadAsync())
            {
                order = new OrderManagementDto
                {
                    OrderGuid = reader.GetGuid(reader.GetOrdinal("OrderGuid")),
                    OrderId = reader.GetInt32(reader.GetOrdinal("OrderId")),
                    CustomerName = reader["CustomerName"].ToString(),
                    CustomerEmail = reader["CustomerEmail"].ToString(),
                    CustomerPhone = reader["CustomerPhone"].ToString(),
                    TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                    OrderStatus = reader["OrderStatus"].ToString(),
                    PaymentStatus = reader["PaymentStatus"].ToString(),
                    PaymentType = reader["PaymentType"].ToString(),
                    OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                    Items = new List<OrderItemDetailDto>()
                };
            }

            if (order != null && await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    order.Items.Add(new OrderItemDetailDto
                    {
                        ProductName = reader["ProductName"].ToString(),
                        Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                        Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                        Total = reader.GetDecimal(reader.GetOrdinal("Total"))
                    });
                }
            }

            return order;
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid orderGuid, string orderStatus, string paymentStatus)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("usp_UpdateOrderStatus", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@OrderGuid", orderGuid);
            cmd.Parameters.AddWithValue("@OrderStatus", orderStatus);
            cmd.Parameters.AddWithValue("@PaymentStatus", paymentStatus);

            await con.OpenAsync();
            int rowsAffected = await cmd.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        // =====================================================
        // PRODUCTS (NEWLY ADDED)
        // =====================================================

        public async Task<List<AdminProductDto>> GetAllProductsForAdminAsync()
        {
            var products = new List<AdminProductDto>();

            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("usp_GetProductsForAdmin", con);
            cmd.CommandType = CommandType.StoredProcedure;

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                products.Add(new AdminProductDto
                {
                    ProductGuid = reader.GetGuid(reader.GetOrdinal("ProductGuid")),
                    Name = reader["Name"].ToString(),
                    Category = reader["Category"].ToString(),
                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                    Unit = reader["Unit"].ToString(),
                    Description = reader["Description"]?.ToString(),
                    ImageUrl = reader["ImageUrl"]?.ToString(),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    TotalSold = reader.GetInt32(reader.GetOrdinal("TotalSold")),
                    TotalRevenue = reader.GetDecimal(reader.GetOrdinal("TotalRevenue"))
                });
            }

            return products;
        }

        public async Task<AdminProductDto?> GetProductWithImagesAsync(Guid productGuid)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("usp_GetProductWithImages", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProductGuid", productGuid);

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            var product = new AdminProductDto
            {
                ProductGuid = reader.GetGuid(reader.GetOrdinal("ProductGuid")),
                Name = reader["Name"].ToString(),
                Category = reader["Category"].ToString(),
                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                Unit = reader["Unit"].ToString(),
                Description = reader["Description"]?.ToString(),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                Images = new List<ProductImageDto>()
            };

            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    product.Images.Add(new ProductImageDto
                    {
                        ImageId = reader.GetInt32(reader.GetOrdinal("ImageId")),
                        ImageUrl = reader["ImageUrl"].ToString(),
                        IsPrimary = reader.GetBoolean(reader.GetOrdinal("IsPrimary"))
                    });
                }
            }

            return product;
        }

        public async Task<Guid> CreateProductAsync(CreateProductDto dto)
        {
            using SqlConnection con = new(_connectionString);
            await con.OpenAsync();
            using SqlTransaction transaction = con.BeginTransaction();

            try
            {
                using SqlCommand cmd = new(@"
                    INSERT INTO Products (Name, Category, Price, Unit, Description, IsActive, CreatedOn)
                    OUTPUT INSERTED.ProductGuid
                    VALUES (@Name, @Category, @Price, @Unit, @Description, 1, GETDATE())",
                    con, transaction);

                cmd.Parameters.AddWithValue("@Name", dto.Name);
                cmd.Parameters.AddWithValue("@Category", dto.Category);
                cmd.Parameters.AddWithValue("@Price", dto.Price);
                cmd.Parameters.AddWithValue("@Unit", dto.Unit);
                cmd.Parameters.AddWithValue("@Description", dto.Description ?? (object)DBNull.Value);

                var productGuid = (Guid)await cmd.ExecuteScalarAsync();

                foreach (var image in dto.Images)
                {
                    using SqlCommand imgCmd = new(@"
                        INSERT INTO ProductImages (ProductGuid, ImageUrl, IsPrimary)
                        VALUES (@ProductGuid, @ImageUrl, @IsPrimary)",
                        con, transaction);

                    imgCmd.Parameters.AddWithValue("@ProductGuid", productGuid);
                    imgCmd.Parameters.AddWithValue("@ImageUrl", image.Base64Image);
                    imgCmd.Parameters.AddWithValue("@IsPrimary", image.IsPrimary);

                    await imgCmd.ExecuteNonQueryAsync();
                }

                transaction.Commit();
                return productGuid;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> UpdateProductAsync(Guid productGuid, UpdateProductDto dto)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new(@"
                UPDATE Products 
                SET 
                    Name = @Name,
                    Category = @Category,
                    Price = @Price,
                    Unit = @Unit,
                    Description = @Description,
                    IsActive = @IsActive
                WHERE ProductGuid = @ProductGuid",
                con);

            cmd.Parameters.AddWithValue("@ProductGuid", productGuid);
            cmd.Parameters.AddWithValue("@Name", dto.Name);
            cmd.Parameters.AddWithValue("@Category", dto.Category);
            cmd.Parameters.AddWithValue("@Price", dto.Price);
            cmd.Parameters.AddWithValue("@Unit", dto.Unit);
            cmd.Parameters.AddWithValue("@Description", dto.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@IsActive", dto.IsActive);

            await con.OpenAsync();
            int rowsAffected = await cmd.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteProductImageAsync(int imageId)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("DELETE FROM ProductImages WHERE ImageId = @ImageId", con);

            cmd.Parameters.AddWithValue("@ImageId", imageId);

            await con.OpenAsync();
            int rowsAffected = await cmd.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> AddProductImageAsync(Guid productGuid, ProductImageUploadDto dto)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new(@"
                INSERT INTO ProductImages (ProductGuid, ImageUrl, IsPrimary)
                VALUES (@ProductGuid, @ImageUrl, @IsPrimary)",
                con);

            cmd.Parameters.AddWithValue("@ProductGuid", productGuid);
            cmd.Parameters.AddWithValue("@ImageUrl", dto.Base64Image);
            cmd.Parameters.AddWithValue("@IsPrimary", dto.IsPrimary);

            await con.OpenAsync();
            int rowsAffected = await cmd.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> ToggleProductStatusAsync(Guid productGuid)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("usp_ToggleProductStatus", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProductGuid", productGuid);

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            return true;
        }


        // Inventory Methods
        public async Task<List<InventoryItemDto>> GetInventoryOverviewAsync()
        {
            var items = new List<InventoryItemDto>();

            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("usp_GetInventoryOverview", con);
            cmd.CommandType = CommandType.StoredProcedure;

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                items.Add(new InventoryItemDto
                {
                    ProductGuid = reader.GetGuid(reader.GetOrdinal("ProductGuid")),
                    ProductName = reader["ProductName"].ToString(),
                    Category = reader["Category"].ToString(),
                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                    StockQuantity = reader.GetInt32(reader.GetOrdinal("StockQuantity")),
                    LowStockThreshold = reader.GetInt32(reader.GetOrdinal("LowStockThreshold")),
                    ImageUrl = reader["ImageUrl"]?.ToString(),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    StockStatus = reader["StockStatus"].ToString(),
                    TotalSold = reader.GetInt32(reader.GetOrdinal("TotalSold")),
                    TotalRevenue = reader.GetDecimal(reader.GetOrdinal("TotalRevenue"))
                });
            }

            return items;
        }

        public async Task<bool> UpdateStockQuantityAsync(Guid productGuid, UpdateStockDto dto)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("usp_UpdateStockQuantity", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProductGuid", productGuid);
            cmd.Parameters.AddWithValue("@QuantityChange", dto.QuantityChange);
            cmd.Parameters.AddWithValue("@ChangeType", dto.ChangeType);
            cmd.Parameters.AddWithValue("@Reason", dto.Reason ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ChangedBy", "Admin");

            await con.OpenAsync();
            try
            {
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<InventoryHistoryDto>> GetInventoryHistoryAsync(Guid? productGuid, int days)
        {
            var history = new List<InventoryHistoryDto>();

            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("usp_GetInventoryHistory", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProductGuid", productGuid.HasValue ? productGuid.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@Days", days);

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                history.Add(new InventoryHistoryDto
                {
                    HistoryId = reader.GetInt32(reader.GetOrdinal("HistoryId")),
                    ProductGuid = reader.GetGuid(reader.GetOrdinal("ProductGuid")),
                    ProductName = reader["ProductName"].ToString(),
                    ChangeType = reader["ChangeType"].ToString(),
                    QuantityChange = reader.GetInt32(reader.GetOrdinal("QuantityChange")),
                    PreviousQuantity = reader.GetInt32(reader.GetOrdinal("PreviousQuantity")),
                    NewQuantity = reader.GetInt32(reader.GetOrdinal("NewQuantity")),
                    Reason = reader["Reason"]?.ToString(),
                    ChangedBy = reader["ChangedBy"].ToString(),
                    ChangedAt = reader.GetDateTime(reader.GetOrdinal("ChangedAt"))
                });
            }

            return history;
        }

        public async Task<List<LowStockAlertDto>> GetLowStockProductsAsync()
        {
            var alerts = new List<LowStockAlertDto>();

            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("usp_GetLowStockProducts", con);
            cmd.CommandType = CommandType.StoredProcedure;

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                alerts.Add(new LowStockAlertDto
                {
                    ProductGuid = reader.GetGuid(reader.GetOrdinal("ProductGuid")),
                    ProductName = reader["ProductName"].ToString(),
                    Category = reader["Category"].ToString(),
                    StockQuantity = reader.GetInt32(reader.GetOrdinal("StockQuantity")),
                    LowStockThreshold = reader.GetInt32(reader.GetOrdinal("LowStockThreshold")),
                    ImageUrl = reader["ImageUrl"]?.ToString(),
                    StockStatus = reader["StockStatus"].ToString()
                });
            }

            return alerts;
        }

        // Order Management Methods
        public async Task<List<OrderDetailDto>> GetOrdersAdvancedAsync(
            int page, int pageSize, string orderStatus, string paymentStatus,
            string paymentType, string searchTerm, DateTime? fromDate, DateTime? toDate)
        {
            var orders = new List<OrderDetailDto>();

            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("usp_GetOrdersAdvanced", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PageNumber", page);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);
            cmd.Parameters.AddWithValue("@OrderStatus", orderStatus ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@PaymentStatus", paymentStatus ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@PaymentType", paymentType ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@SearchTerm", searchTerm ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@FromDate", fromDate.HasValue ? fromDate.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@ToDate", toDate.HasValue ? toDate.Value : DBNull.Value);

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                orders.Add(new OrderDetailDto
                {
                    OrderGuid = reader.GetGuid(reader.GetOrdinal("OrderGuid")),
                    OrderId = reader.GetInt32(reader.GetOrdinal("OrderId")),
                    CustomerName = reader["CustomerName"].ToString(),
                    CustomerEmail = reader["CustomerEmail"].ToString(),
                    CustomerPhone = reader["CustomerPhone"]?.ToString(),
                    Subtotal = reader.GetDecimal(reader.GetOrdinal("Subtotal")),
                    Tax = reader.GetDecimal(reader.GetOrdinal("Tax")),
                    Shipping = reader.GetDecimal(reader.GetOrdinal("Shipping")),
                    TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                    OrderStatus = reader["OrderStatus"].ToString(),
                    PaymentStatus = reader["PaymentStatus"].ToString(),
                    PaymentType = reader["PaymentType"].ToString(),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    ShippingAddress = reader["ShippingAddress"]?.ToString(),
                    ItemCount = reader.GetInt32(reader.GetOrdinal("ItemCount"))
                });
            }

            return orders;
        }

        public async Task<OrderDetailDto?> GetOrderDetailsCompleteAsync(Guid orderGuid)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("usp_GetOrderDetailsComplete", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@OrderGuid", orderGuid);

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            OrderDetailDto order = null;

            // Read order info
            if (await reader.ReadAsync())
            {
                order = new OrderDetailDto
                {
                    OrderGuid = reader.GetGuid(reader.GetOrdinal("OrderGuid")),
                    OrderId = reader.GetInt32(reader.GetOrdinal("OrderId")),
                    CustomerName = reader.IsDBNull(reader.GetOrdinal("CustomerName"))
                        ? string.Empty
                        : reader["CustomerName"].ToString(),
                    CustomerEmail = reader.IsDBNull(reader.GetOrdinal("CustomerEmail"))
                        ? string.Empty
                        : reader["CustomerEmail"].ToString(),
                    CustomerPhone = reader.IsDBNull(reader.GetOrdinal("CustomerPhone"))
                        ? null
                        : reader["CustomerPhone"].ToString(),
                    Subtotal = reader.IsDBNull(reader.GetOrdinal("Subtotal"))
                        ? 0
                        : reader.GetDecimal(reader.GetOrdinal("Subtotal")),
                    Tax = reader.IsDBNull(reader.GetOrdinal("Tax"))
                        ? 0
                        : reader.GetDecimal(reader.GetOrdinal("Tax")),
                    Shipping = reader.IsDBNull(reader.GetOrdinal("Shipping"))
                        ? 0
                        : reader.GetDecimal(reader.GetOrdinal("Shipping")),
                    TotalAmount = reader.IsDBNull(reader.GetOrdinal("TotalAmount"))
                        ? 0
                        : reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                    OrderStatus = reader.IsDBNull(reader.GetOrdinal("OrderStatus"))
                        ? "Unknown"
                        : reader["OrderStatus"].ToString(),
                    PaymentStatus = reader.IsDBNull(reader.GetOrdinal("PaymentStatus"))
                        ? "Unknown"
                        : reader["PaymentStatus"].ToString(),
                    PaymentType = reader.IsDBNull(reader.GetOrdinal("PaymentType"))
                        ? "Unknown"
                        : reader["PaymentType"].ToString(),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    ShippingAddress = reader.IsDBNull(reader.GetOrdinal("ShippingAddress"))
                        ? null
                        : reader["ShippingAddress"].ToString(),
                    BillingAddress = reader.IsDBNull(reader.GetOrdinal("BillingAddress"))
                        ? null
                        : reader["BillingAddress"].ToString(),
                    DeliveryInstructions = reader.IsDBNull(reader.GetOrdinal("DeliveryInstructions"))
                        ? null
                        : reader["DeliveryInstructions"].ToString(),
                    OrderNotes = reader.IsDBNull(reader.GetOrdinal("OrderNotes"))
                        ? null
                        : reader["OrderNotes"].ToString()
                };
            }

            // Read order items
            if (order != null && await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    order.Items.Add(new OrderItemDto
                    {
                        OrderItemId = reader.GetInt32(reader.GetOrdinal("OrderItemId")),
                        ProductGuid = reader.GetGuid(reader.GetOrdinal("ProductGuid")),
                        ProductName = reader.IsDBNull(reader.GetOrdinal("ProductName"))
                            ? string.Empty
                            : reader["ProductName"].ToString(),
                        Quantity = reader.IsDBNull(reader.GetOrdinal("Quantity"))
                            ? 0
                            : reader.GetInt32(reader.GetOrdinal("Quantity")),
                        Price = reader.IsDBNull(reader.GetOrdinal("Price"))
                            ? 0
                            : reader.GetDecimal(reader.GetOrdinal("Price")),
                        Subtotal = reader.IsDBNull(reader.GetOrdinal("Subtotal"))
                            ? 0
                            : reader.GetDecimal(reader.GetOrdinal("Subtotal")),
                        ImageUrl = reader.IsDBNull(reader.GetOrdinal("ImageUrl"))
                            ? null
                            : reader["ImageUrl"].ToString(),
                        StockQuantity = reader.IsDBNull(reader.GetOrdinal("StockQuantity"))
                            ? 0
                            : reader.GetInt32(reader.GetOrdinal("StockQuantity"))
                    });
                }
            }

            return order;
        }


        public async Task<OrderStatisticsDto> GetOrderStatisticsAsync(int days)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("usp_GetOrderStatistics", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Days", days);

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new OrderStatisticsDto
                {
                    TotalOrders = reader.GetInt32(reader.GetOrdinal("TotalOrders")),
                    PendingOrders = reader.GetInt32(reader.GetOrdinal("PendingOrders")),
                    ProcessingOrders = reader.GetInt32(reader.GetOrdinal("ProcessingOrders")),
                    ShippedOrders = reader.GetInt32(reader.GetOrdinal("ShippedOrders")),
                    DeliveredOrders = reader.GetInt32(reader.GetOrdinal("DeliveredOrders")),
                    CancelledOrders = reader.GetInt32(reader.GetOrdinal("CancelledOrders")),
                    PaidOrders = reader.GetInt32(reader.GetOrdinal("PaidOrders")),
                    UnpaidOrders = reader.GetInt32(reader.GetOrdinal("UnpaidOrders")),
                    CODOrders = reader.GetInt32(reader.GetOrdinal("CODOrders")),
                    OnlineOrders = reader.GetInt32(reader.GetOrdinal("OnlineOrders")),
                    TotalRevenue = reader.GetDecimal(reader.GetOrdinal("TotalRevenue")),
                    AverageOrderValue = reader.GetDecimal(reader.GetOrdinal("AverageOrderValue"))
                };
            }

            return new OrderStatisticsDto();
        }

        public async Task<int> BulkUpdateOrderStatusAsync(List<string> orderGuids, string orderStatus, string paymentStatus)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("usp_BulkUpdateOrderStatus", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@OrderGuids", string.Join(",", orderGuids));
            cmd.Parameters.AddWithValue("@OrderStatus", orderStatus);
            cmd.Parameters.AddWithValue("@PaymentStatus", paymentStatus ?? (object)DBNull.Value);

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return reader.GetInt32(reader.GetOrdinal("UpdatedCount"));
            }

            return 0;
        }


        // Get Customers with Advanced Filters
        // =====================================================
        // CUSTOMER MANAGEMENT
        // =====================================================

        public async Task<CustomerListResponse> GetCustomersAdvancedAsync(
            int page,
            int pageSize,
            string searchTerm,
            string customerStatus,
            DateTime? fromDate,
            DateTime? toDate,
            int? minOrders,
            int? maxOrders,
            decimal? minSpending,
            decimal? maxSpending)
        {
            var customers = new List<CustomerDto>();
            int totalCount = 0;

            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("sp_GetCustomersAdvanced", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Page", page);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);
            cmd.Parameters.AddWithValue("@SearchTerm", searchTerm ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CustomerStatus", customerStatus ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@FromDate", fromDate.HasValue ? fromDate.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@ToDate", toDate.HasValue ? toDate.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@MinOrders", minOrders.HasValue ? minOrders.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@MaxOrders", maxOrders.HasValue ? maxOrders.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@MinSpending", minSpending.HasValue ? minSpending.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@MaxSpending", maxSpending.HasValue ? maxSpending.Value : DBNull.Value);

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                customers.Add(new CustomerDto
                {
                    UserGuid = reader.GetGuid(reader.GetOrdinal("UserGuid")),
                    Email = reader["Email"].ToString(),
                    Name = reader["Name"].ToString(),
                    PhoneNumber = reader["PhoneNumber"]?.ToString(),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    LastLoginAt = reader.IsDBNull(reader.GetOrdinal("LastLoginAt"))
                        ? null
                        : reader.GetDateTime(reader.GetOrdinal("LastLoginAt")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    TotalOrders = reader.GetInt32(reader.GetOrdinal("TotalOrders")),
                    TotalSpent = reader.GetDecimal(reader.GetOrdinal("TotalSpent")),
                    LastOrderDate = reader.IsDBNull(reader.GetOrdinal("LastOrderDate"))
                        ? null
                        : reader.GetDateTime(reader.GetOrdinal("LastOrderDate"))
                });

                if (totalCount == 0 && !reader.IsDBNull(reader.GetOrdinal("TotalCount")))
                {
                    totalCount = reader.GetInt32(reader.GetOrdinal("TotalCount"));
                }
            }

            return new CustomerListResponse
            {
                Customers = customers,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<CustomerDetailDto?> GetCustomerDetailsCompleteAsync(Guid userGuid)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("sp_GetCustomerDetailsComplete", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserGuid", userGuid);

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            CustomerDetailDto customer = null;

            // Read customer info
            if (await reader.ReadAsync())
            {
                customer = new CustomerDetailDto
                {
                    UserGuid = reader.GetGuid(reader.GetOrdinal("UserGuid")),
                    Email = reader["Email"].ToString(),
                    Name = reader["Name"].ToString(),
                    PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber"))
                        ? null
                        : reader["PhoneNumber"].ToString(),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    LastLoginAt = reader.IsDBNull(reader.GetOrdinal("LastLoginAt"))
                        ? null
                        : reader.GetDateTime(reader.GetOrdinal("LastLoginAt")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),  // BIT -> bool
                    Role = reader["Role"].ToString(),
                    TotalOrders = reader.GetInt32(reader.GetOrdinal("TotalOrders")),
                    TotalSpent = reader.GetDecimal(reader.GetOrdinal("TotalSpent")),
                    AverageOrderValue = reader.GetDecimal(reader.GetOrdinal("AverageOrderValue")),
                    LastOrderDate = reader.IsDBNull(reader.GetOrdinal("LastOrderDate"))
                        ? null
                        : reader.GetDateTime(reader.GetOrdinal("LastOrderDate")),
                    OrdersLast30Days = reader.GetInt32(reader.GetOrdinal("OrdersLast30Days")),
                    OrdersLast90Days = reader.GetInt32(reader.GetOrdinal("OrdersLast90Days")),
                    CustomerStatus = reader["CustomerStatus"].ToString()
                };
            }

            if (customer == null)
                return null;

            // Read addresses
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    customer.Addresses.Add(new CustomerAddressDto
                    {
                        AddressGuid = reader.GetGuid(reader.GetOrdinal("AddressGuid")),
                        AddressLine1 = reader["AddressLine1"].ToString(),
                        AddressLine2 = reader.IsDBNull(reader.GetOrdinal("AddressLine2"))
                            ? null
                            : reader["AddressLine2"].ToString(),
                        City = reader.IsDBNull(reader.GetOrdinal("City"))
                            ? null
                            : reader["City"].ToString(),
                        State = reader.IsDBNull(reader.GetOrdinal("State"))
                            ? null
                            : reader["State"].ToString(),
                        PostalCode = reader.IsDBNull(reader.GetOrdinal("PostalCode"))
                            ? null
                            : reader["PostalCode"].ToString(),
                        Country = reader["Country"].ToString(),
                        IsDefault = reader.GetBoolean(reader.GetOrdinal("IsDefault")),  // BIT -> bool
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                    });
                }
            }

            // Read recent orders
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    customer.RecentOrders.Add(new CustomerOrderDto
                    {
                        OrderGuid = reader.GetGuid(reader.GetOrdinal("OrderGuid")),
                        OrderId = reader.GetInt32(reader.GetOrdinal("OrderId")).ToString(),
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                        TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                        OrderStatus = reader.IsDBNull(reader.GetOrdinal("OrderStatus"))
                            ? "Unknown"
                            : reader["OrderStatus"].ToString(),
                        PaymentStatus = reader.IsDBNull(reader.GetOrdinal("PaymentStatus"))
                            ? "Unknown"
                            : reader["PaymentStatus"].ToString(),
                        PaymentType = reader.IsDBNull(reader.GetOrdinal("PaymentType"))
                            ? "Unknown"
                            : reader["PaymentType"].ToString(),
                        ItemCount = reader.GetInt32(reader.GetOrdinal("ItemCount"))
                    });
                }
            }

            return customer;
        }

        public async Task<CustomerStatisticsDto> GetCustomerStatisticsAsync()
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("sp_GetCustomerStatistics", con);

            cmd.CommandType = CommandType.StoredProcedure;

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new CustomerStatisticsDto
                {
                    TotalCustomers = reader.GetInt32(reader.GetOrdinal("TotalCustomers")),
                    NewCustomers = reader.GetInt32(reader.GetOrdinal("NewCustomers")),
                    ActiveCustomers = reader.GetInt32(reader.GetOrdinal("ActiveCustomers")),
                    InactiveCustomers = reader.GetInt32(reader.GetOrdinal("InactiveCustomers"))
                };
            }

            return new CustomerStatisticsDto();
        }

        public async Task<bool> ToggleCustomerStatusAsync(Guid userGuid)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("sp_ToggleCustomerStatus", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserGuid", userGuid);

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            return await reader.ReadAsync();
        }

        public async Task<List<CustomerValueSegmentDto>> GetCustomerValueSegmentsAsync()
        {
            var segments = new List<CustomerValueSegmentDto>();

            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("sp_GetCustomerValueSegments", con);

            cmd.CommandType = CommandType.StoredProcedure;

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                segments.Add(new CustomerValueSegmentDto
                {
                    Segment = reader["Segment"].ToString(),
                    CustomerCount = reader.GetInt32(reader.GetOrdinal("CustomerCount")),
                    AverageValue = reader.GetDecimal(reader.GetOrdinal("AverageValue")),
                    TotalValue = reader.GetDecimal(reader.GetOrdinal("TotalValue"))
                });
            }

            return segments;
        }

        public async Task<List<TopCustomerDto>> GetTopCustomersAsync(int topCount, string orderBy)
        {
            var customers = new List<TopCustomerDto>();

            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("sp_GetTopCustomers", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TopCount", topCount);
            cmd.Parameters.AddWithValue("@OrderBy", orderBy);

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                customers.Add(new TopCustomerDto
                {
                    UserGuid = reader.GetGuid(reader.GetOrdinal("UserGuid")),
                    Name = reader["Name"].ToString(),
                    Email = reader["Email"].ToString(),
                    PhoneNumber = reader["PhoneNumber"]?.ToString(),
                    TotalOrders = reader.GetInt32(reader.GetOrdinal("TotalOrders")),
                    TotalSpent = reader.GetDecimal(reader.GetOrdinal("TotalSpent")),
                    AverageOrderValue = reader.GetDecimal(reader.GetOrdinal("AverageOrderValue")),
                    LastOrderDate = reader.IsDBNull(reader.GetOrdinal("LastOrderDate"))
                        ? null
                        : reader.GetDateTime(reader.GetOrdinal("LastOrderDate"))
                });
            }

            return customers;
        }

        public async Task<List<CustomerDto>> ExportCustomerDataAsync()
        {
            var customers = new List<CustomerDto>();

            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("sp_ExportCustomerData", con);

            cmd.CommandType = CommandType.StoredProcedure;

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                customers.Add(new CustomerDto
                {
                    UserGuid = reader.GetGuid(reader.GetOrdinal("UserGuid")),
                    Email = reader["Email"].ToString(),
                    Name = reader["Name"].ToString(),
                    PhoneNumber = reader["PhoneNumber"]?.ToString(),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    LastLoginAt = reader.IsDBNull(reader.GetOrdinal("LastLoginAt"))
                        ? null
                        : reader.GetDateTime(reader.GetOrdinal("LastLoginAt")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    TotalOrders = reader.GetInt32(reader.GetOrdinal("TotalOrders")),
                    TotalSpent = reader.GetDecimal(reader.GetOrdinal("TotalSpent")),
                    LastOrderDate = reader.IsDBNull(reader.GetOrdinal("LastOrderDate"))
                        ? null
                        : reader.GetDateTime(reader.GetOrdinal("LastOrderDate"))
                });
            }

            return customers;
        }

        // Add these methods to your existing AdminRepository class

        // =====================================================
        // ANALYTICS & REPORTS
        // =====================================================

        public async Task<SalesAnalyticsDto> GetSalesAnalyticsAsync(int days)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("sp_GetSalesAnalytics", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Days", days);

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            var analytics = new SalesAnalyticsDto();

            // Daily Sales
            while (await reader.ReadAsync())
            {
                analytics.DailySales.Add(new DailySalesDto
                {
                    Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                    OrderCount = reader.GetInt32(reader.GetOrdinal("OrderCount")),
                    Revenue = reader.GetDecimal(reader.GetOrdinal("Revenue")),
                    AverageOrderValue = reader.GetDecimal(reader.GetOrdinal("AverageOrderValue")),
                    UniqueCustomers = reader.GetInt32(reader.GetOrdinal("UniqueCustomers"))
                });
            }

            // Summary
            if (await reader.NextResultAsync() && await reader.ReadAsync())
            {
                analytics.Summary = new SalesSummaryDto
                {
                    TotalOrders = reader.GetInt32(reader.GetOrdinal("TotalOrders")),
                    TotalRevenue = reader.GetDecimal(reader.GetOrdinal("TotalRevenue")),
                    AverageOrderValue = reader.GetDecimal(reader.GetOrdinal("AverageOrderValue")),
                    TotalCustomers = reader.GetInt32(reader.GetOrdinal("TotalCustomers")),
                    DeliveredRevenue = reader.GetDecimal(reader.GetOrdinal("DeliveredRevenue")),
                    CancelledRevenue = reader.GetDecimal(reader.GetOrdinal("CancelledRevenue"))
                };
            }

            // Hourly Pattern
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    analytics.HourlyPattern.Add(new HourlyPatternDto
                    {
                        Hour = reader.GetInt32(reader.GetOrdinal("Hour")),
                        OrderCount = reader.GetInt32(reader.GetOrdinal("OrderCount")),
                        Revenue = reader.GetDecimal(reader.GetOrdinal("Revenue"))
                    });
                }
            }

            // Day of Week Pattern
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    analytics.DayOfWeekPattern.Add(new DayOfWeekPatternDto
                    {
                        DayOfWeek = reader["DayOfWeek"].ToString(),
                        DayNumber = reader.GetInt32(reader.GetOrdinal("DayNumber")),
                        OrderCount = reader.GetInt32(reader.GetOrdinal("OrderCount")),
                        Revenue = reader.GetDecimal(reader.GetOrdinal("Revenue"))
                    });
                }
            }

            return analytics;
        }

        public async Task<ProductAnalyticsDto> GetProductAnalyticsAsync(int days)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("sp_GetProductAnalytics", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Days", days);

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            var analytics = new ProductAnalyticsDto();

            // Top Products
            while (await reader.ReadAsync())
            {
                analytics.TopProducts.Add(new TopProductDto
                {
                    ProductGuid = reader.GetGuid(reader.GetOrdinal("ProductGuid")),
                    ProductName = reader["ProductName"].ToString(),
                    Category = reader["Category"].ToString(),
                    TotalSold = reader.GetInt32(reader.GetOrdinal("TotalQuantity")),
                    TotalRevenue = reader.GetDecimal(reader.GetOrdinal("TotalRevenue"))
                });
            }

            // Category Performance
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    analytics.CategoryPerformance.Add(new CategoryPerformanceDto
                    {
                        Category = reader["Category"].ToString(),
                        ProductCount = reader.GetInt32(reader.GetOrdinal("ProductCount")),
                        TotalSales = reader.GetInt32(reader.GetOrdinal("TotalSales")),
                        TotalQuantity = reader.GetInt32(reader.GetOrdinal("TotalQuantity")),
                        TotalRevenue = reader.GetDecimal(reader.GetOrdinal("TotalRevenue")),
                        AveragePrice = reader.GetDecimal(reader.GetOrdinal("AveragePrice"))
                    });
                }
            }

            // Low Performers
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    analytics.LowPerformers.Add(new LowPerformerDto
                    {
                        ProductGuid = reader.GetGuid(reader.GetOrdinal("ProductGuid")),
                        ProductName = reader["ProductName"].ToString(),
                        Category = reader["Category"].ToString(),
                        Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                        TimesSold = reader.GetInt32(reader.GetOrdinal("TimesSold")),
                        TotalQuantity = reader.GetInt32(reader.GetOrdinal("TotalQuantity")),
                        TotalRevenue = reader.GetDecimal(reader.GetOrdinal("TotalRevenue"))
                    });
                }
            }

            return analytics;
        }

        public async Task<CustomerAnalyticsDto> GetCustomerAnalyticsAsync(int days)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("sp_GetCustomerAnalytics", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Days", days);

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            var analytics = new CustomerAnalyticsDto();

            // Customer Acquisition
            while (await reader.ReadAsync())
            {
                analytics.CustomerAcquisition.Add(new CustomerAcquisitionDto
                {
                    Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                    NewCustomers = reader.GetInt32(reader.GetOrdinal("NewCustomers"))
                });
            }

            // Retention Metrics
            if (await reader.NextResultAsync() && await reader.ReadAsync())
            {
                analytics.Retention = new CustomerRetentionDto
                {
                    ActiveCustomers = reader.GetInt32(reader.GetOrdinal("ActiveCustomers")),
                    ReturningCustomers = reader.GetInt32(reader.GetOrdinal("ReturningCustomers")),
                    NewCustomersWithOrders = reader.GetInt32(reader.GetOrdinal("NewCustomersWithOrders"))
                };
            }

            // Value Distribution
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    analytics.ValueDistribution.Add(new CustomerValueDistributionDto
                    {
                        ValueSegment = reader["ValueSegment"].ToString(),
                        CustomerCount = reader.GetInt32(reader.GetOrdinal("CustomerCount")),
                        AverageValue = reader.GetDecimal(reader.GetOrdinal("AverageValue")),
                        TotalValue = reader.GetDecimal(reader.GetOrdinal("TotalValue"))
                    });
                }
            }

            // Geographic Distribution
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    analytics.GeographicDistribution.Add(new GeographicDistributionDto
                    {
                        City = reader["City"].ToString(),
                        State = reader["State"].ToString(),
                        CustomerCount = reader.GetInt32(reader.GetOrdinal("CustomerCount")),
                        OrderCount = reader.GetInt32(reader.GetOrdinal("OrderCount")),
                        TotalRevenue = reader.GetDecimal(reader.GetOrdinal("TotalRevenue"))
                    });
                }
            }

            return analytics;
        }

        public async Task<RevenueReportDto> GetRevenueReportsAsync(DateTime startDate, DateTime endDate)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("sp_GetRevenueReports", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@StartDate", startDate);
            cmd.Parameters.AddWithValue("@EndDate", endDate);

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            var report = new RevenueReportDto();

            // Daily Revenue
            while (await reader.ReadAsync())
            {
                report.DailyRevenue.Add(new DailyRevenueDto
                {
                    Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                    TotalRevenue = reader.GetDecimal(reader.GetOrdinal("TotalRevenue")),
                    Subtotal = reader.GetDecimal(reader.GetOrdinal("Subtotal")),
                    Tax = reader.GetDecimal(reader.GetOrdinal("Tax")),
                    Shipping = reader.GetDecimal(reader.GetOrdinal("Shipping")),
                    OrderCount = reader.GetInt32(reader.GetOrdinal("OrderCount"))
                });
            }

            // Payment Method Breakdown
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    report.PaymentMethods.Add(new PaymentMethodBreakdownDto
                    {
                        PaymentType = reader["PaymentType"].ToString(),
                        OrderCount = reader.GetInt32(reader.GetOrdinal("OrderCount")),
                        TotalRevenue = reader.GetDecimal(reader.GetOrdinal("TotalRevenue")),
                        AverageOrderValue = reader.GetDecimal(reader.GetOrdinal("AverageOrderValue"))
                    });
                }
            }

            // Category Revenue
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    report.CategoryRevenue.Add(new CategoryRevenueDto
                    {
                        Category = reader["Category"].ToString(),
                        Revenue = reader.GetDecimal(reader.GetOrdinal("Revenue")),
                        OrderCount = reader.GetInt32(reader.GetOrdinal("OrderCount")),
                        TotalQuantity = reader.GetInt32(reader.GetOrdinal("TotalQuantity"))
                    });
                }
            }

            // Summary
            if (await reader.NextResultAsync() && await reader.ReadAsync())
            {
                report.Summary = new RevenueSummaryDto
                {
                    TotalRevenue = reader.GetDecimal(reader.GetOrdinal("TotalRevenue")),
                    TotalSubtotal = reader.GetDecimal(reader.GetOrdinal("TotalSubtotal")),
                    TotalTax = reader.GetDecimal(reader.GetOrdinal("TotalTax")),
                    TotalShipping = reader.GetDecimal(reader.GetOrdinal("TotalShipping")),
                    TotalOrders = reader.GetInt32(reader.GetOrdinal("TotalOrders")),
                    AverageOrderValue = reader.GetDecimal(reader.GetOrdinal("AverageOrderValue")),
                    UniqueCustomers = reader.GetInt32(reader.GetOrdinal("UniqueCustomers"))
                };
            }

            return report;
        }

        public async Task<InventoryReportDto> GetInventoryReportsAsync()
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("sp_GetInventoryReports", con);

            cmd.CommandType = CommandType.StoredProcedure;

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            var report = new InventoryReportDto();

            // Stock Status
            while (await reader.ReadAsync())
            {
                report.StockStatus.Add(new StockStatusDto
                {
                    ProductGuid = reader.GetGuid(reader.GetOrdinal("ProductGuid")),
                    ProductName = reader["ProductName"].ToString(),
                    Category = reader["Category"].ToString(),
                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                    StockQuantity = reader.GetInt32(reader.GetOrdinal("StockQuantity")),
                    LowStockThreshold = reader.GetInt32(reader.GetOrdinal("LowStockThreshold")),
                    StockStatus = reader["StockStatus"].ToString(),
                    StockValue = reader.GetDecimal(reader.GetOrdinal("StockValue"))
                });
            }

            // Stock Movement
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    report.StockMovement.Add(new StockMovementDto
                    {
                        ProductGuid = reader.GetGuid(reader.GetOrdinal("ProductGuid")),
                        ProductName = reader["ProductName"].ToString(),
                        ChangeType = reader["ChangeType"].ToString(),
                        TotalChange = reader.GetInt32(reader.GetOrdinal("TotalChange")),
                        TransactionCount = reader.GetInt32(reader.GetOrdinal("TransactionCount"))
                    });
                }
            }

            // Alerts Summary
            if (await reader.NextResultAsync() && await reader.ReadAsync())
            {
                report.AlertsSummary = new StockAlertsSummaryDto
                {
                    OutOfStockCount = reader.GetInt32(reader.GetOrdinal("OutOfStockCount")),
                    LowStockCount = reader.GetInt32(reader.GetOrdinal("LowStockCount")),
                    InStockCount = reader.GetInt32(reader.GetOrdinal("InStockCount")),
                    TotalStockValue = reader.GetDecimal(reader.GetOrdinal("TotalStockValue"))
                };
            }

            return report;
        }

        public async Task<OrderFulfillmentReportDto> GetOrderFulfillmentReportAsync(int days)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("sp_GetOrderFulfillmentReport", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Days", days);

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            var report = new OrderFulfillmentReportDto();

            // Status Breakdown
            while (await reader.ReadAsync())
            {
                report.StatusBreakdown.Add(new OrderStatusBreakdownDto
                {
                    OrderStatus = reader["OrderStatus"].ToString(),
                    OrderCount = reader.GetInt32(reader.GetOrdinal("OrderCount")),
                    Revenue = reader.GetDecimal(reader.GetOrdinal("Revenue")),
                    AverageDays = reader.GetDecimal(reader.GetOrdinal("AverageDays"))
                });
            }

            // Delivery Performance
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    report.DeliveryPerformance.Add(new DeliveryPerformanceDto
                    {
                        Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                        DeliveredOrders = reader.GetInt32(reader.GetOrdinal("DeliveredOrders")),
                        CancelledOrders = reader.GetInt32(reader.GetOrdinal("CancelledOrders")),
                        PendingOrders = reader.GetInt32(reader.GetOrdinal("PendingOrders"))
                    });
                }
            }

            // Fulfillment Time
            if (await reader.NextResultAsync() && await reader.ReadAsync())
            {
                report.FulfillmentTime = new FulfillmentTimeDto
                {
                    AverageFulfillmentDays = reader.GetDecimal(reader.GetOrdinal("AverageFulfillmentDays")),
                    MinFulfillmentDays = reader.GetDecimal(reader.GetOrdinal("MinFulfillmentDays")),
                    MaxFulfillmentDays = reader.GetDecimal(reader.GetOrdinal("MaxFulfillmentDays"))
                };
            }

            return report;
        }

        public async Task<ComparisonReportDto> GetComparisonReportAsync(
            DateTime period1Start, DateTime period1End,
            DateTime period2Start, DateTime period2End)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("sp_GetComparisonReport", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Period1Start", period1Start);
            cmd.Parameters.AddWithValue("@Period1End", period1End);
            cmd.Parameters.AddWithValue("@Period2Start", period2Start);
            cmd.Parameters.AddWithValue("@Period2End", period2End);

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            var report = new ComparisonReportDto();

            if (await reader.ReadAsync())
            {
                report.Period1 = new PeriodMetricsDto
                {
                    TotalOrders = reader.GetInt32(reader.GetOrdinal("TotalOrders")),
                    TotalRevenue = reader.GetDecimal(reader.GetOrdinal("TotalRevenue")),
                    AverageOrderValue = reader.GetDecimal(reader.GetOrdinal("AverageOrderValue")),
                    UniqueCustomers = reader.GetInt32(reader.GetOrdinal("UniqueCustomers")),
                    TotalItemsSold = reader.GetInt32(reader.GetOrdinal("TotalItemsSold"))
                };
            }

            if (await reader.ReadAsync())
            {
                report.Period2 = new PeriodMetricsDto
                {
                    TotalOrders = reader.GetInt32(reader.GetOrdinal("TotalOrders")),
                    TotalRevenue = reader.GetDecimal(reader.GetOrdinal("TotalRevenue")),
                    AverageOrderValue = reader.GetDecimal(reader.GetOrdinal("AverageOrderValue")),
                    UniqueCustomers = reader.GetInt32(reader.GetOrdinal("UniqueCustomers")),
                    TotalItemsSold = reader.GetInt32(reader.GetOrdinal("TotalItemsSold"))
                };
            }

            return report;
        }

        public async Task<ExecutiveSummaryDto> GetExecutiveSummaryAsync()
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("sp_GetExecutiveSummary", con);

            cmd.CommandType = CommandType.StoredProcedure;

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            var summary = new ExecutiveSummaryDto();

            // Overall Metrics
            if (await reader.ReadAsync())
            {
                summary.OverallMetrics = new OverallMetricsDto
                {
                    TotalOrders = reader.GetInt32(reader.GetOrdinal("TotalOrders")),
                    TotalRevenue = reader.GetDecimal(reader.GetOrdinal("TotalRevenue")),
                    AverageOrderValue = reader.GetDecimal(reader.GetOrdinal("AverageOrderValue")),
                    TotalCustomers = reader.GetInt32(reader.GetOrdinal("TotalCustomers")),
                    RegisteredUsers = reader.GetInt32(reader.GetOrdinal("RegisteredUsers")),
                    TotalProducts = reader.GetInt32(reader.GetOrdinal("TotalProducts")),
                    ActiveProducts = reader.GetInt32(reader.GetOrdinal("ActiveProducts")),
                    OrdersLast30Days = reader.GetInt32(reader.GetOrdinal("OrdersLast30Days")),
                    RevenueLast30Days = reader.GetDecimal(reader.GetOrdinal("RevenueLast30Days")),
                    NewCustomersLast30Days = reader.GetInt32(reader.GetOrdinal("NewCustomersLast30Days"))
                };
            }

            // Top Products
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    summary.TopProducts.Add(new TopProductSummaryDto
                    {
                        ProductName = reader["ProductName"].ToString(),
                        Category = reader["Category"].ToString(),
                        TotalSold = reader.GetInt32(reader.GetOrdinal("TotalSold")),
                        TotalRevenue = reader.GetDecimal(reader.GetOrdinal("TotalRevenue"))
                    });
                }
            }

            // Growth Metrics
            if (await reader.NextResultAsync() && await reader.ReadAsync())
            {
                var recentRevenue = reader.GetDecimal(reader.GetOrdinal("RecentRevenue"));
                var previousRevenue = reader.GetDecimal(reader.GetOrdinal("PreviousRevenue"));
                var recentOrders = reader.GetInt32(reader.GetOrdinal("RecentOrders"));
                var previousOrders = reader.GetInt32(reader.GetOrdinal("PreviousOrders"));

                summary.GrowthMetrics = new GrowthMetricsDto
                {
                    RecentRevenue = recentRevenue,
                    PreviousRevenue = previousRevenue,
                    RecentOrders = recentOrders,
                    PreviousOrders = previousOrders,
                    RevenueGrowth = previousRevenue > 0
                        ? ((recentRevenue - previousRevenue) / previousRevenue) * 100
                        : 0,
                    OrderGrowth = previousOrders > 0
                        ? ((recentOrders - previousOrders) / (decimal)previousOrders) * 100
                        : 0
                };
            }

            return summary;
        }
 

        // =====================================================
        // EMAIL NOTIFICATIONS
        // =====================================================

        public async Task<List<EmailNotificationDto>> GetPendingEmailNotificationsAsync(int top = 50)
        {
            var notifications = new List<EmailNotificationDto>();

            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("sp_GetPendingEmailNotifications", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Top", top);

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                notifications.Add(new EmailNotificationDto
                {
                    NotificationGuid = reader.GetGuid(reader.GetOrdinal("NotificationGuid")),
                    RecipientEmail = reader["RecipientEmail"].ToString(),
                    RecipientName = reader["RecipientName"]?.ToString(),
                    Subject = reader["Subject"].ToString(),
                    Body = reader["Body"].ToString(),
                    NotificationType = reader["NotificationType"].ToString(),
                    Status = reader["Status"].ToString(),
                    Priority = reader["Priority"].ToString(),
                    ScheduledAt = reader.IsDBNull(reader.GetOrdinal("ScheduledAt"))
                        ? null
                        : reader.GetDateTime(reader.GetOrdinal("ScheduledAt")),
                    RetryCount = reader.GetInt32(reader.GetOrdinal("RetryCount")),
                    RelatedEntityId = reader["RelatedEntityId"]?.ToString(),
                    RelatedEntityType = reader["RelatedEntityType"]?.ToString(),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                });
            }

            return notifications;
        }

        public async Task<EmailNotificationListResponse> GetEmailNotificationHistoryAsync(
            int days,
            string status,
            string notificationType,
            int page,
            int pageSize)
        {
            var notifications = new List<EmailNotificationDto>();
            int totalCount = 0;

            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("sp_GetEmailNotificationHistory", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Days", days);
            cmd.Parameters.AddWithValue("@Status", status ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@NotificationType", notificationType ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Page", page);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                notifications.Add(new EmailNotificationDto
                {
                    NotificationGuid = reader.GetGuid(reader.GetOrdinal("NotificationGuid")),
                    RecipientEmail = reader["RecipientEmail"].ToString(),
                    RecipientName = reader["RecipientName"]?.ToString(),
                    Subject = reader["Subject"].ToString(),
                    NotificationType = reader["NotificationType"].ToString(),
                    Status = reader["Status"].ToString(),
                    Priority = reader["Priority"].ToString(),
                    ScheduledAt = reader.IsDBNull(reader.GetOrdinal("ScheduledAt"))
                        ? null
                        : reader.GetDateTime(reader.GetOrdinal("ScheduledAt")),
                    SentAt = reader.IsDBNull(reader.GetOrdinal("SentAt"))
                        ? null
                        : reader.GetDateTime(reader.GetOrdinal("SentAt")),
                    FailureReason = reader["FailureReason"]?.ToString(),
                    RetryCount = reader.GetInt32(reader.GetOrdinal("RetryCount")),
                    RelatedEntityId = reader["RelatedEntityId"]?.ToString(),
                    RelatedEntityType = reader["RelatedEntityType"]?.ToString(),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                });

                if (totalCount == 0 && !reader.IsDBNull(reader.GetOrdinal("TotalCount")))
                {
                    totalCount = reader.GetInt32(reader.GetOrdinal("TotalCount"));
                }
            }

            return new EmailNotificationListResponse
            {
                Notifications = notifications,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<EmailStatisticsDto> GetEmailNotificationStatisticsAsync(int days)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("sp_GetEmailNotificationStatistics", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Days", days);

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            var statistics = new EmailStatisticsDto();

            // Overview
            if (await reader.ReadAsync())
            {
                statistics.Overview = new EmailStatsOverview
                {
                    TotalNotifications = reader.GetInt32(reader.GetOrdinal("TotalNotifications")),
                    SentCount = reader.GetInt32(reader.GetOrdinal("SentCount")),
                    FailedCount = reader.GetInt32(reader.GetOrdinal("FailedCount")),
                    PendingCount = reader.GetInt32(reader.GetOrdinal("PendingCount")),
                    SuccessRate = reader.IsDBNull(reader.GetOrdinal("SuccessRate"))
                        ? 0
                        : reader.GetDecimal(reader.GetOrdinal("SuccessRate")),
                    AvgDeliveryTimeSeconds = reader["AvgDeliveryTimeSeconds"] == DBNull.Value
    ? 0
    : Convert.ToDecimal(reader["AvgDeliveryTimeSeconds"])
                };
            }
 

            // By Type
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    statistics.ByType.Add(new EmailStatsByType
                    {
                        NotificationType = reader["NotificationType"].ToString(),
                        TotalCount = reader.GetInt32(reader.GetOrdinal("TotalCount")),
                        SentCount = reader.GetInt32(reader.GetOrdinal("SentCount")),
                        FailedCount = reader.GetInt32(reader.GetOrdinal("FailedCount")),
                        PendingCount = reader.GetInt32(reader.GetOrdinal("PendingCount"))
                    });
                }
            }

            // Daily Trend
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    statistics.DailyTrend.Add(new EmailStatsByDay
                    {
                        Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                        TotalCount = reader.GetInt32(reader.GetOrdinal("TotalCount")),
                        SentCount = reader.GetInt32(reader.GetOrdinal("SentCount")),
                        FailedCount = reader.GetInt32(reader.GetOrdinal("FailedCount"))
                    });
                }
            }

            return statistics;
        }

        public async Task<int> CreateEmailNotificationAsync(CreateEmailNotificationDto dto)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("sp_CreateEmailNotification", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RecipientEmail", dto.RecipientEmail);
            cmd.Parameters.AddWithValue("@RecipientName", dto.RecipientName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Subject", dto.Subject);
            cmd.Parameters.AddWithValue("@Body", dto.Body);
            cmd.Parameters.AddWithValue("@NotificationType", dto.NotificationType);
            cmd.Parameters.AddWithValue("@Priority", dto.Priority);
            cmd.Parameters.AddWithValue("@ScheduledAt", dto.ScheduledAt ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@RelatedEntityId", dto.RelatedEntityId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@RelatedEntityType", dto.RelatedEntityType ?? (object)DBNull.Value);

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return reader.GetInt32(reader.GetOrdinal("NotificationId"));
            }

            return 0;
        }

        public async Task<bool> UpdateEmailNotificationStatusAsync(
            Guid notificationGuid,
            string status,
            string failureReason)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("sp_UpdateEmailNotificationStatus", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@NotificationGuid", notificationGuid);
            cmd.Parameters.AddWithValue("@Status", status);
            cmd.Parameters.AddWithValue("@FailureReason", failureReason ?? (object)DBNull.Value);

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return reader.GetInt32(reader.GetOrdinal("RowsAffected")) > 0;
            }

            return false;
        }

        public async Task<List<EmailTemplateDto>> GetEmailTemplatesAsync()
        {
            var templates = new List<EmailTemplateDto>();

            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("sp_GetEmailTemplates", con);

            cmd.CommandType = CommandType.StoredProcedure;

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                templates.Add(new EmailTemplateDto
                {
                    TemplateGuid = reader.GetGuid(reader.GetOrdinal("TemplateGuid")),
                    TemplateName = reader["TemplateName"].ToString(),
                    Subject = reader["Subject"].ToString(),
                    BodyTemplate = reader["BodyTemplate"].ToString(),
                    Description = reader["Description"]?.ToString(),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                });
            }

            return templates;
        }

        public async Task<EmailTemplateDto?> GetEmailTemplateByNameAsync(string templateName)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("sp_GetEmailTemplateByName", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TemplateName", templateName);

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new EmailTemplateDto
                {
                    TemplateGuid = reader.GetGuid(reader.GetOrdinal("TemplateGuid")),
                    TemplateName = reader["TemplateName"].ToString(),
                    Subject = reader["Subject"].ToString(),
                    BodyTemplate = reader["BodyTemplate"].ToString(),
                    Description = reader["Description"]?.ToString(),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                };
            }

            return null;
        }

        public async Task<bool> SaveEmailTemplateAsync(SaveEmailTemplateDto dto)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("sp_SaveEmailTemplate", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TemplateName", dto.TemplateName);
            cmd.Parameters.AddWithValue("@Subject", dto.Subject);
            cmd.Parameters.AddWithValue("@BodyTemplate", dto.BodyTemplate);
            cmd.Parameters.AddWithValue("@Description", dto.Description ?? (object)DBNull.Value);

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return reader.GetInt32(reader.GetOrdinal("RowsAffected")) > 0;
            }

            return false;
        }



        public async Task<bool> SendOrderConfirmationEmailAsync(int orderId)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("sp_SendOrderConfirmationEmail", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@OrderId", orderId);

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            return true;
        }



    }
}