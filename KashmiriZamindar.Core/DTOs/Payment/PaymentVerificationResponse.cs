namespace KashmiriZamindar.Core.Dtos.Payment
{
    public class PaymentVerificationResponse
    {
        public bool IsValid { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public int OrderId { get; set; }
        public string OrderGuid { get; set; }
        public decimal AmountPaid { get; set; }
    }
}