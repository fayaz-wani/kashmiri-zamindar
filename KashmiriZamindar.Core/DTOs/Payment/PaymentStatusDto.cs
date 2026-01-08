// 7. Core/Dtos/Payment/PaymentStatusDto.cs
// ============================================
namespace KashmiriZamindar.Core.Dtos.Payment
{
    public class PaymentStatusDto
    {
        public int OrderId { get; set; }
        public string OrderGuid { get; set; }
        public string PaymentStatus { get; set; }
        public string OrderStatus { get; set; }
        public string TransactionId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}