namespace KashmiriZamindar.Core.Dtos.Payment
{
    public class RazorpayOrderResponse
    {
        public string RazorpayOrderId { get; set; }
        public string RazorpayKeyId { get; set; }      // Public key for frontend
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string OrderGuid { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
    }
}
