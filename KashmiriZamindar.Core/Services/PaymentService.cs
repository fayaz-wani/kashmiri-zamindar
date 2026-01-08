using KashmiriZamindar.Core.Dtos.Payment;
using KashmiriZamindar.Core.Entities;
using KashmiriZamindar.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Net.Http;
using System.Text;
using System.Text.Json;


namespace KashmiriZamindar.Core.Services
{
    public class PaymentService
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly IAdminRepository _adminRepo;
        private readonly ICheckoutRepository _checkoutRepo;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        private string RazorpayKeyId => _configuration["Razorpay:KeyId"];
        private string RazorpayKeySecret => _configuration["Razorpay:KeySecret"];

        public PaymentService(
            IPaymentRepository paymentRepo, IAdminRepository adminRepo,
            ICheckoutRepository checkoutRepo,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _paymentRepo = paymentRepo;
            _adminRepo = adminRepo;
            _checkoutRepo = checkoutRepo;
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();

            // Setup basic auth for Razorpay
            var authToken = Convert.ToBase64String(
                Encoding.ASCII.GetBytes($"{RazorpayKeyId}:{RazorpayKeySecret}")
            );
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);

            Console.WriteLine($"💳 RAZORPAY INITIALIZED - Key: {RazorpayKeyId}");
        }

        public async Task<RazorpayOrderResponse> CreateRazorpayOrderAsync(CreatePaymentOrderDto dto)
        {
            try
            {
                var order = await _checkoutRepo.GetOrderByIdAsync(dto.OrderId);
                if (order == null)
                    throw new Exception("Order not found");

                // Prepare Razorpay request
                var razorpayRequest = new
                {
                    amount = (int)(dto.Amount * 100), // Convert to paise
                    currency = dto.Currency,
                    receipt = $"order_{order.OrderGuid.ToString().Substring(0, 8)}",
                    notes = new
                    {
                        order_id = order.OrderId.ToString(),
                        customer_name = dto.CustomerName,
                        customer_email = dto.CustomerEmail
                    }
                };

                var jsonContent = JsonSerializer.Serialize(razorpayRequest);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                Console.WriteLine($"📤 Creating Razorpay order for OrderId: {dto.OrderId}, Amount: ₹{dto.Amount}");

                var response = await _httpClient.PostAsync(
                    "https://api.razorpay.com/v1/orders",
                    content
                );

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Razorpay API error: {errorContent}");
                    throw new Exception($"Razorpay API error: {errorContent}");
                }

                var responseJson = await response.Content.ReadAsStringAsync();
                var razorpayOrder = JsonSerializer.Deserialize<JsonElement>(responseJson);
                string razorpayOrderId = razorpayOrder.GetProperty("id").GetString();

                Console.WriteLine($"✅ Razorpay order created: {razorpayOrderId}");

                // Save payment transaction
                var transaction = new PaymentTransaction
                {
                    OrderId = order.OrderId,
                    PaymentGateway = "Razorpay",
                    RazorpayOrderId = razorpayOrderId,
                    Amount = dto.Amount,
                    Currency = dto.Currency,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                };

                await _paymentRepo.CreateTransactionAsync(transaction);

                return new RazorpayOrderResponse
                {
                    RazorpayOrderId = razorpayOrderId,
                    RazorpayKeyId = RazorpayKeyId,
                    Amount = dto.Amount,
                    Currency = dto.Currency,
                    OrderGuid = order.OrderGuid.ToString(),
                    CustomerName = dto.CustomerName,
                    CustomerEmail = dto.CustomerEmail,
                    CustomerPhone = dto.CustomerPhone
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error creating Razorpay order: {ex.Message}");
                throw new Exception($"Failed to create Razorpay order: {ex.Message}");
            }
        }

        // Replace the entire VerifyPaymentAsync method in PaymentService.cs

        // ✅ UPDATE VerifyPaymentAsync method
        public async Task<PaymentVerificationResponse> VerifyPaymentAsync(VerifyPaymentDto dto)
        {
            try
            {
                Console.WriteLine($"🔍 Verifying payment for order: {dto.RazorpayOrderId}");

                var transaction = await _paymentRepo.GetByRazorpayOrderIdAsync(dto.RazorpayOrderId);
                if (transaction == null)
                {
                    Console.WriteLine($"❌ Transaction not found for order: {dto.RazorpayOrderId}");
                    return new PaymentVerificationResponse
                    {
                        IsValid = false,
                        Status = "Failed",
                        Message = "Payment transaction not found"
                    };
                }

                bool isValid = await _paymentRepo.VerifyPaymentSignatureAsync(
                    dto.RazorpayOrderId,
                    dto.RazorpayPaymentId,
                    dto.RazorpaySignature,
                    RazorpayKeySecret
                );

                if (!isValid)
                {
                    Console.WriteLine($"❌ Signature verification failed");
                    transaction.Status = "Failed";
                    transaction.ErrorMessage = "Signature verification failed";
                    await _paymentRepo.UpdateTransactionAsync(transaction);

                    return new PaymentVerificationResponse
                    {
                        IsValid = false,
                        Status = "Failed",
                        Message = "Payment verification failed"
                    };
                }

                Console.WriteLine($"✅ Payment verified successfully");

                transaction.RazorpayPaymentId = dto.RazorpayPaymentId;
                transaction.RazorpaySignature = dto.RazorpaySignature;
                transaction.Status = "Success";
                transaction.CompletedAt = DateTime.UtcNow;
                await _paymentRepo.UpdateTransactionAsync(transaction);

                await _checkoutRepo.UpdateOrderPaymentStatusAsync(transaction.OrderId, "Paid");

                // ✅ SEND EMAIL USING REPOSITORY
                try
                {
                    await _adminRepo.SendOrderConfirmationEmailAsync(transaction.OrderId);
                    Console.WriteLine($"✅ Order confirmation email queued for Order #{transaction.OrderId}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Failed to queue email: {ex.Message}");
                }

                var order = await _checkoutRepo.GetOrderByIdAsync(transaction.OrderId);

                return new PaymentVerificationResponse
                {
                    IsValid = true,
                    Status = "Success",
                    Message = "Payment verified successfully",
                    OrderId = order.OrderId,
                    OrderGuid = order.OrderGuid.ToString(),
                    AmountPaid = transaction.Amount
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Payment verification error: {ex.Message}");
                return new PaymentVerificationResponse
                {
                    IsValid = false,
                    Status = "Failed",
                    Message = $"Verification error: {ex.Message}"
                };
            }
        }
        public async Task<PaymentStatusDto> GetPaymentStatusAsync(int orderId)
        {
            var order = await _checkoutRepo.GetOrderByIdAsync(orderId);
            if (order == null)
                throw new Exception("Order not found");

            var transaction = await _paymentRepo.GetByOrderIdAsync(orderId);

            return new PaymentStatusDto
            {
                OrderId = order.OrderId,
                OrderGuid = order.OrderGuid.ToString(),
                PaymentStatus = order.PaymentStatus,
                OrderStatus = order.OrderStatus,
                TransactionId = transaction?.RazorpayPaymentId,
                TotalAmount = order.TotalAmount,
                PaidAt = transaction?.CompletedAt
            };
        }
    }
}