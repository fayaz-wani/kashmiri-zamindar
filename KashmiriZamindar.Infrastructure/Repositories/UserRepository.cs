// UserRepository.cs - Add these methods

using KashmiriZamindar.Core.Dtos;
using KashmiriZamindar.Core.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
namespace KashmiriZamindar.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevConnection");
        }
        public async Task<List<UserOrderDto>> GetUserOrdersAsync(Guid userGuid, int page, int pageSize)
        {
            var orders = new List<UserOrderDto>();

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_GetUserOrders", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserGuid", userGuid);
            cmd.Parameters.AddWithValue("@Page", page);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);

            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                orders.Add(new UserOrderDto
                {
                    OrderGuid = reader.GetGuid(reader.GetOrdinal("OrderGuid")),
                    OrderId = reader.GetInt32(reader.GetOrdinal("OrderId")),
                    TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                    OrderStatus = reader["OrderStatus"].ToString(),
                    PaymentStatus = reader["PaymentStatus"].ToString(),
                    PaymentType = reader["PaymentType"].ToString(),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    ShippingAddress = reader["ShippingAddress"].ToString(),
                    ItemCount = reader.GetInt32(reader.GetOrdinal("ItemCount"))
                });
            }

            return orders;
        }
        public async Task<UserOrderDetailDto?> GetUserOrderDetailsAsync(Guid orderGuid, Guid userGuid)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_GetUserOrderDetails", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@OrderGuid", orderGuid);
            cmd.Parameters.AddWithValue("@UserGuid", userGuid);

            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            UserOrderDetailDto? order = null;

            // Read order info
            if (await reader.ReadAsync())
            {
                order = new UserOrderDetailDto
                {
                    OrderGuid = reader.GetGuid(reader.GetOrdinal("OrderGuid")),
                    OrderId = reader.GetInt32(reader.GetOrdinal("OrderId")),
                    CustomerEmail = reader["CustomerEmail"]?.ToString() ?? "",
                    CustomerPhone = reader["CustomerPhone"]?.ToString() ?? "",
                    ShippingAddress = reader["ShippingAddress"]?.ToString() ?? "",
                    BillingAddress = reader["BillingAddress"]?.ToString() ?? "",
                    PaymentType = reader["PaymentType"]?.ToString() ?? "",
                    PaymentStatus = reader["PaymentStatus"]?.ToString() ?? "",
                    OrderStatus = reader["OrderStatus"]?.ToString() ?? "",
                    Subtotal = reader.IsDBNull(reader.GetOrdinal("Subtotal")) ? 0 : reader.GetDecimal(reader.GetOrdinal("Subtotal")),
                    Tax = reader.IsDBNull(reader.GetOrdinal("Tax")) ? 0 : reader.GetDecimal(reader.GetOrdinal("Tax")),
                    ShippingCost = reader.IsDBNull(reader.GetOrdinal("ShippingCost")) ? 0 : reader.GetDecimal(reader.GetOrdinal("ShippingCost")),
                    TotalAmount = reader.IsDBNull(reader.GetOrdinal("TotalAmount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                    CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                    Items = new List<UserOrderItemDto>() // IMPORTANT: initialize
                };
            }

            // Read order items
            if (order != null && await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    order.Items.Add(new UserOrderItemDto
                    {
                        OrderItemId = reader.GetInt32(reader.GetOrdinal("OrderItemId")),
                        ProductGuid = reader.GetGuid(reader.GetOrdinal("ProductGuid")),
                        ProductName = reader["ProductName"]?.ToString() ?? "",
                        Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                        Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                        Subtotal = reader.GetDecimal(reader.GetOrdinal("Subtotal")),
                        ImageUrl = reader["ImageUrl"]?.ToString() ?? "assets/images/placeholder.jpg"
                    });
                }
            }

            return order; // null handled by controller
        }

    }
}