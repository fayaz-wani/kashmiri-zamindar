// Core/Services/CheckoutService.cs
using KashmiriZamindar.Core.Dtos;
using KashmiriZamindar.Core.Dtos.Payment;
using KashmiriZamindar.Core.Entities;
using KashmiriZamindar.Core.Interfaces;

namespace KashmiriZamindar.Core.Services
{
    public class CheckoutService
    {
        private readonly ICheckoutRepository _checkoutRepo;
        private readonly PaymentService _paymentService;

        public CheckoutService(
            ICheckoutRepository checkoutRepo,
            PaymentService paymentService)
        {
            _checkoutRepo = checkoutRepo;
            _paymentService = paymentService;
        }

        public async Task<CheckoutResponseDto> ProcessCheckoutAsync(CheckoutRequestDto request)
        {
            try
            {
                // 1️⃣ Create Customer (if new)
                var customer = new Customer
                {
                    FirstName = request.ContactInfo.FirstName,
                    LastName = request.ContactInfo.LastName,
                    Email = request.ContactInfo.Email,
                    Phone = request.ContactInfo.Phone,
                    CreatedAt = DateTime.UtcNow
                };
                var customerId = await _checkoutRepo.CreateOrGetCustomerAsync(customer);

                // 2️⃣ Create Order
                var order = new Order
                {
                    OrderGuid = Guid.NewGuid(),
                    CustomerId = customerId,
                    CustomerEmail = request.ContactInfo.Email,
                    CustomerPhone = request.ContactInfo.Phone,

                    ShippingAddress = $"{request.ShippingAddress.Address1}, {request.ShippingAddress.Address2}, " +
                                    $"{request.ShippingAddress.City}, {request.ShippingAddress.State} - {request.ShippingAddress.Zip}",

                    BillingAddress = $"{request.PaymentInfo.BillingAddress.Address1}, {request.PaymentInfo.BillingAddress.City}, " +
                                    $"{request.PaymentInfo.BillingAddress.State} - {request.PaymentInfo.BillingAddress.Zip}",

                    PaymentType = request.PaymentInfo.PaymentType,
                    PaymentStatus = request.PaymentInfo.PaymentType?.ToUpper() == "COD" ? "Pending" : "Awaiting Payment",
                    OrderStatus = "Pending",

                    Subtotal = request.Subtotal,
                    Tax = request.Tax,
                    ShippingCost = request.Shipping,
                    TotalAmount = request.Total,

                    CreatedAt = DateTime.UtcNow
                };

                int orderId = await _checkoutRepo.CreateOrderAsync(order);

                // 3️⃣ Create Order Items
                foreach (var item in request.OrderItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = orderId,
                        ProductGuid = item.ProductGuid,
                        ProductName = item.ProductName,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        Subtotal = item.Price * item.Quantity
                    };
                    await _checkoutRepo.CreateOrderItemAsync(orderItem);
                }

                // 4️⃣ Handle Payment Based on Type
                var response = new CheckoutResponseDto
                {
                    OrderId = orderId,
                    OrderGuid = order.OrderGuid.ToString(),
                    Message = "Order created successfully"
                };

                // ✅ Check if payment requires online processing (UPI, Card, Online)
                var paymentType = request.PaymentInfo.PaymentType?.ToUpper();
                var requiresOnlinePayment = paymentType == "ONLINE" ||
                                          paymentType == "CARD" ||
                                          paymentType == "UPI";

                if (requiresOnlinePayment)
                {
                    var paymentOrderDto = new CreatePaymentOrderDto
                    {
                        OrderId = orderId,
                        Amount = request.Total,
                        Currency = "INR",
                        CustomerName = $"{request.ContactInfo.FirstName} {request.ContactInfo.LastName}",
                        CustomerEmail = request.ContactInfo.Email,
                        CustomerPhone = request.ContactInfo.Phone
                    };

                    var razorpayOrder = await _paymentService.CreateRazorpayOrderAsync(paymentOrderDto);

                    response.RequiresPayment = true;
                    response.PaymentDetails = razorpayOrder;
                    response.Message = "Order created. Please complete payment.";
                }
                else
                {
                    // COD - No payment required
                    response.RequiresPayment = false;
                    response.Message = "Order placed successfully with Cash on Delivery";
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"Checkout failed: {ex.Message}");
            }
        }
    }
}