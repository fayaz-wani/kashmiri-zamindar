namespace KashmiriZamindar.Core.Entities
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public Guid ProductGuid { get; set; }  // ✅ Changed from ProductId
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Subtotal { get; set; }  // ✅ Added
    }
}