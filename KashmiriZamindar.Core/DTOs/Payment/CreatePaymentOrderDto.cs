// ============================================
// 3. Core/Dtos/Payment/CreatePaymentOrderDto.cs
// ============================================
namespace KashmiriZamindar.Core.Dtos.Payment
{
    public class CreatePaymentOrderDto
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
    }
}