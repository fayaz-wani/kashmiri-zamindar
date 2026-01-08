// 5. Core/Dtos/Payment/VerifyPaymentDto.cs
// ============================================


namespace KashmiriZamindar.Core.Dtos.Payment
{

    using System.Text.Json.Serialization;

    public class VerifyPaymentDto
    {
        public int OrderId { get; set; }
        public string RazorpayOrderId { get; set; }
        public string RazorpayPaymentId { get; set; }
        public string RazorpaySignature { get; set; }
    }


}