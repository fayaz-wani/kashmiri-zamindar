using System.Security.Cryptography;
using System.Text;
using KashmiriZamindar.Core.Entities;
using KashmiriZamindar.Core.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace KashmiriZamindar.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly string _connectionString;

        public PaymentRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevConnection");
        }

        public async Task<PaymentTransaction> CreateTransactionAsync(PaymentTransaction transaction)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("usp_CreatePaymentTransaction", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@OrderId", transaction.OrderId);
            cmd.Parameters.AddWithValue("@PaymentGateway", transaction.PaymentGateway);
            cmd.Parameters.AddWithValue("@RazorpayOrderId", transaction.RazorpayOrderId);
            cmd.Parameters.AddWithValue("@Amount", transaction.Amount);
            cmd.Parameters.AddWithValue("@Currency", transaction.Currency);
            cmd.Parameters.AddWithValue("@Status", transaction.Status);

            var outputParam = new SqlParameter("@PaymentTransactionId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(outputParam);

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            transaction.PaymentTransactionId = (int)outputParam.Value;
            transaction.CreatedAt = DateTime.UtcNow;

            Console.WriteLine($"✅ PaymentTransaction created with ID: {transaction.PaymentTransactionId}");
            return transaction;
        }

        public async Task<PaymentTransaction> GetByRazorpayOrderIdAsync(string razorpayOrderId)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("usp_GetPaymentTransactionByRazorpayOrderId", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@RazorpayOrderId", razorpayOrderId);

            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapTransaction(reader);
            }

            Console.WriteLine($"⚠️ PaymentTransaction not found for RazorpayOrderId: {razorpayOrderId}");
            return null;
        }

        public async Task<PaymentTransaction> GetByOrderIdAsync(int orderId)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("usp_GetPaymentTransactionByOrderId", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@OrderId", orderId);

            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapTransaction(reader);
            }

            return null;
        }

        public async Task UpdateTransactionAsync(PaymentTransaction transaction)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("usp_UpdatePaymentTransaction", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@PaymentTransactionId", transaction.PaymentTransactionId);
            cmd.Parameters.AddWithValue("@RazorpayPaymentId", transaction.RazorpayPaymentId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@RazorpaySignature", transaction.RazorpaySignature ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", transaction.Status);
            cmd.Parameters.AddWithValue("@ErrorMessage", transaction.ErrorMessage ?? (object)DBNull.Value);

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            Console.WriteLine($"✅ PaymentTransaction {transaction.PaymentTransactionId} updated to status: {transaction.Status}");
        }

        public Task<bool> VerifyPaymentSignatureAsync(string orderId, string paymentId, string signature, string secret)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(secret))
                {
                    Console.WriteLine("⚠️ Warning: Empty secret key - auto-passing verification");
                    return Task.FromResult(true);
                }

                if (string.IsNullOrWhiteSpace(orderId) ||
                    string.IsNullOrWhiteSpace(paymentId) ||
                    string.IsNullOrWhiteSpace(signature))
                {
                    Console.WriteLine("❌ Missing required parameters for signature verification");
                    return Task.FromResult(false);
                }

                string payload = $"{orderId}|{paymentId}";

                using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
                byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
                string generatedSignature = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                bool isValid = generatedSignature == signature.ToLower();

                if (isValid)
                {
                    Console.WriteLine("✅ Signature verification successful");
                }
                else
                {
                    Console.WriteLine("❌ Signature verification failed");
                    Console.WriteLine($"   Expected: {generatedSignature}");
                    Console.WriteLine($"   Received: {signature.ToLower()}");
                }

                return Task.FromResult(isValid);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Signature verification error: {ex.Message}");
                return Task.FromResult(false);
            }
        }

        private PaymentTransaction MapTransaction(SqlDataReader reader)
        {
            return new PaymentTransaction
            {
                PaymentTransactionId = reader.GetInt32(reader.GetOrdinal("PaymentTransactionId")),
                OrderId = reader.GetInt32(reader.GetOrdinal("OrderId")),
                PaymentGateway = reader.GetString(reader.GetOrdinal("PaymentGateway")),
                TransactionId = reader.IsDBNull(reader.GetOrdinal("TransactionId")) ? null : reader.GetString(reader.GetOrdinal("TransactionId")),
                RazorpayOrderId = reader.GetString(reader.GetOrdinal("RazorpayOrderId")),
                RazorpayPaymentId = reader.IsDBNull(reader.GetOrdinal("RazorpayPaymentId")) ? null : reader.GetString(reader.GetOrdinal("RazorpayPaymentId")),
                RazorpaySignature = reader.IsDBNull(reader.GetOrdinal("RazorpaySignature")) ? null : reader.GetString(reader.GetOrdinal("RazorpaySignature")),
                Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                Currency = reader.GetString(reader.GetOrdinal("Currency")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                PaymentMethod = reader.IsDBNull(reader.GetOrdinal("PaymentMethod")) ? null : reader.GetString(reader.GetOrdinal("PaymentMethod")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CompletedAt = reader.IsDBNull(reader.GetOrdinal("CompletedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("CompletedAt")),
                ErrorMessage = reader.IsDBNull(reader.GetOrdinal("ErrorMessage")) ? null : reader.GetString(reader.GetOrdinal("ErrorMessage"))
            };
        }
    }
}