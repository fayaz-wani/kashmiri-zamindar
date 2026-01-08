namespace KashmiriZamindar.Core.Entities
{
    public class PaymentTransaction
    {
        public int PaymentTransactionId { get; set; }
        public int OrderId { get; set; }
        public string PaymentGateway { get; set; }
        public string TransactionId { get; set; }
        public string RazorpayOrderId { get; set; }
        public string RazorpayPaymentId { get; set; }
        public string RazorpaySignature { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        public string Status { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string ErrorMessage { get; set; }

        // Navigation
        public Order Order { get; set; }
    }
}