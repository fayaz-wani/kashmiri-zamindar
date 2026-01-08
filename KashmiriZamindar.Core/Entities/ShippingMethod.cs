namespace KashmiriZamindar.Core.Entities
{
    public class ShippingMethod
    {
        public int ShippingMethodId { get; set; } // PK
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public string EstimatedDelivery { get; set; }
    }
}
