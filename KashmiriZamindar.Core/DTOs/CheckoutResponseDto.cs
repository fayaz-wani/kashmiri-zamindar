// 9. Core/Dtos/Checkout/CheckoutResponseDto.cs (Updated)
// ============================================
using KashmiriZamindar.Core.Dtos.Payment;

namespace KashmiriZamindar.Core.Dtos
{
    public class CheckoutResponseDto
    {
        public int OrderId { get; set; }
        public string OrderGuid { get; set; }
        public string Message { get; set; }

        // For online payment
        public bool RequiresPayment { get; set; }
        public RazorpayOrderResponse PaymentDetails { get; set; }
    }
}