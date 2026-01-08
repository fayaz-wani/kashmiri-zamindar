using KashmiriZamindar.Core.Entities;
using KashmiriZamindar.Core.Interfaces;
using KashmiriZamindar.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Threading.Tasks;

namespace KashmiriZamindar.Infrastructure.Repositories
{
    public class CheckoutRepository : ICheckoutRepository
    {
        private readonly string _connectionString;
        private readonly IAdminRepository _adminRepo;

        public CheckoutRepository(IConfiguration configuration,IAdminRepository adminRepo)
        {
            _connectionString = configuration.GetConnectionString("DevConnection");
            _adminRepo = adminRepo;
        }

        public async Task<int> CreateOrGetCustomerAsync(Customer customer)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("usp_CreateOrGetCustomer", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@FirstName", customer.FirstName);
            cmd.Parameters.AddWithValue("@LastName", customer.LastName);
            cmd.Parameters.AddWithValue("@Email", customer.Email);
            cmd.Parameters.AddWithValue("@Phone", customer.Phone ?? "");

            var output = new SqlParameter("@CustomerId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(output);

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            return (int)output.Value;
        }

        // Replace the entire CreateOrderAsync method in CheckoutRepository.cs

        public async Task<int> CreateOrderAsync(Order order)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("usp_CreateOrder", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@OrderGuid", order.OrderGuid);
            cmd.Parameters.AddWithValue("@CustomerId", order.CustomerId);
            cmd.Parameters.AddWithValue("@CustomerEmail", order.CustomerEmail);
            cmd.Parameters.AddWithValue("@CustomerPhone", order.CustomerPhone);
            cmd.Parameters.AddWithValue("@ShippingAddress", order.ShippingAddress);
            cmd.Parameters.AddWithValue("@BillingAddress", order.BillingAddress);
            cmd.Parameters.AddWithValue("@PaymentType", order.PaymentType);
            cmd.Parameters.AddWithValue("@PaymentStatus", order.PaymentStatus);
            cmd.Parameters.AddWithValue("@OrderStatus", order.OrderStatus);
            cmd.Parameters.AddWithValue("@Subtotal", order.Subtotal);
            cmd.Parameters.AddWithValue("@Tax", order.Tax);
            cmd.Parameters.AddWithValue("@ShippingCost", order.ShippingCost);
            cmd.Parameters.AddWithValue("@TotalAmount", order.TotalAmount);

            var output = new SqlParameter("@OrderId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(output);

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            int orderId = (int)output.Value;

            // ✅ SEND EMAIL FOR COD ORDERS USING REPOSITORY
            if (order.PaymentType?.ToUpper() == "COD")
            {
                try
                {
                    await _adminRepo.SendOrderConfirmationEmailAsync(orderId);
                    Console.WriteLine($"✅ Order confirmation email queued for Order #{orderId}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Failed to queue email for Order #{orderId}: {ex.Message}");
                }
            }

            return orderId;
        }

        public async Task CreateOrderItemAsync(OrderItem orderItem)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("usp_CreateOrderItem", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@OrderId", orderItem.OrderId);
            cmd.Parameters.AddWithValue("@ProductGuid", orderItem.ProductGuid);
            cmd.Parameters.AddWithValue("@ProductName", orderItem.ProductName);
            cmd.Parameters.AddWithValue("@Quantity", orderItem.Quantity);
            cmd.Parameters.AddWithValue("@Price", orderItem.Price);
            cmd.Parameters.AddWithValue("@Subtotal", orderItem.Subtotal);

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("usp_GetOrderById", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@OrderId", orderId);

            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Order
                {
                    OrderId = reader.GetInt32(reader.GetOrdinal("OrderId")),
                    OrderGuid = reader.GetGuid(reader.GetOrdinal("OrderGuid")),
                    CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                    CustomerEmail = reader.GetString(reader.GetOrdinal("CustomerEmail")),
                    CustomerPhone = reader.GetString(reader.GetOrdinal("CustomerPhone")),
                    ShippingAddress = reader.GetString(reader.GetOrdinal("ShippingAddress")),
                    BillingAddress = reader.GetString(reader.GetOrdinal("BillingAddress")),
                    PaymentType = reader.GetString(reader.GetOrdinal("PaymentType")),
                    PaymentStatus = reader.GetString(reader.GetOrdinal("PaymentStatus")),
                    OrderStatus = reader.GetString(reader.GetOrdinal("OrderStatus")),
                    Subtotal = reader.GetDecimal(reader.GetOrdinal("Subtotal")),
                    Tax = reader.GetDecimal(reader.GetOrdinal("Tax")),
                    ShippingCost = reader.GetDecimal(reader.GetOrdinal("ShippingCost")),
                    TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                };
            }

            return null;
        }

        public async Task UpdateOrderPaymentStatusAsync(int orderId, string paymentStatus)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("usp_UpdateOrderPaymentStatus", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@OrderId", orderId);
            cmd.Parameters.AddWithValue("@PaymentStatus", paymentStatus);

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
