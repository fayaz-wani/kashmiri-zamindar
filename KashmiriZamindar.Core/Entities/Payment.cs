using System;

namespace KashmiriZamindar.Core.Entities
{
    public class Payment
    {
        public int PaymentId { get; set; } // PK
        public int OrderId { get; set; }   // FK
        public string PaymentType { get; set; } // card, applepay, googlepay
        public string CardNumber { get; set; }
        public string CardExpiry { get; set; }
        public string CardCVV { get; set; }
        public string NameOnCard { get; set; }
        public string BillingAddress { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
