using System.Data;
using Microsoft.Data.SqlClient;
using KashmiriZamindar.Core.Dtos;
using KashmiriZamindar.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace KashmiriZamindar.Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly string _connectionString;

        public CartRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevConnection");
        }

        public async Task<CartDto> GetCartAsync(string sessionId, Guid? userGuid)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("usp_GetCart", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SessionId", sessionId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@UserGuid", userGuid.HasValue ? userGuid.Value : DBNull.Value);

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            var cart = new CartDto
            {
                SessionId = sessionId,
                UserGuid = userGuid
            };

            while (await reader.ReadAsync())
            {
                var item = new CartItemDto
                {
                    CartItemId = reader.GetInt32(reader.GetOrdinal("CartItemId")),
                    ProductGuid = reader.GetGuid(reader.GetOrdinal("ProductGuid")),
                    ProductName = reader["ProductName"].ToString(),
                    ProductCategory = reader["ProductCategory"].ToString(),
                    ProductUnit = reader["ProductUnit"].ToString(),
                    ImageUrl = reader["ImageUrl"]?.ToString(),
                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                    Price = reader.GetDecimal(reader.GetOrdinal("Price"))
                };

                item.ItemTotal = item.Price * item.Quantity;
                cart.Items.Add(item);
            }

            cart.Subtotal = cart.Items.Sum(i => i.ItemTotal);
            cart.TotalItems = cart.Items.Sum(i => i.Quantity);

            return cart;
        }

        public async Task<CartItemDto> AddToCartAsync(AddToCartDto dto)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("usp_AddToCart", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SessionId", dto.SessionId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@UserGuid", dto.UserGuid.HasValue ? dto.UserGuid.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@ProductGuid", dto.ProductGuid);
            cmd.Parameters.AddWithValue("@Quantity", dto.Quantity);

            await con.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                throw new Exception("Failed to add item to cart");

            return new CartItemDto
            {
                CartItemId = reader.GetInt32(reader.GetOrdinal("CartItemId")),
                ProductGuid = reader.GetGuid(reader.GetOrdinal("ProductGuid")),
                ProductName = reader["ProductName"].ToString(),
                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                Price = reader.GetDecimal(reader.GetOrdinal("Price"))
            };
        }

        public async Task<bool> UpdateCartItemQuantityAsync(string sessionId, Guid? userGuid, int cartItemId, int quantity)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("usp_UpdateCartItemQuantity", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SessionId", sessionId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@UserGuid", userGuid.HasValue ? userGuid.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@CartItemId", cartItemId);
            cmd.Parameters.AddWithValue("@Quantity", quantity);

            await con.OpenAsync();
            int rowsAffected = await cmd.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> RemoveCartItemAsync(string sessionId, Guid? userGuid, int cartItemId)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("usp_RemoveCartItem", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SessionId", sessionId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@UserGuid", userGuid.HasValue ? userGuid.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@CartItemId", cartItemId);

            await con.OpenAsync();
            int rowsAffected = await cmd.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> ClearCartAsync(string sessionId, Guid? userGuid)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("usp_ClearCart", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SessionId", sessionId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@UserGuid", userGuid.HasValue ? userGuid.Value : DBNull.Value);

            await con.OpenAsync();
            int rowsAffected = await cmd.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> MergeGuestCartToUserAsync(string guestSessionId, Guid userGuid)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new("usp_MergeGuestCartToUser", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@GuestSessionId", guestSessionId);
            cmd.Parameters.AddWithValue("@UserGuid", userGuid);

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            return true;
        }
    }
}