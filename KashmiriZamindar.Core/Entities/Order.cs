// ============================================
// 2. Core/Entities/Order.cs (Updated)
// ============================================
namespace KashmiriZamindar.Core.Entities
{
    public class Order
    {
        public int OrderId { get; set; }
        public Guid OrderGuid { get; set; }             // Public facing ID
        public int CustomerId { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string ShippingAddress { get; set; }
        public string BillingAddress { get; set; }
        public string PaymentType { get; set; }         // COD, Online, Card
        public string PaymentStatus { get; set; }       // Pending, Paid, Failed
        public string OrderStatus { get; set; }         // Pending, Confirmed, Shipped, Delivered
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public Customer Customer { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public ICollection<PaymentTransaction> PaymentTransactions { get; set; }
    }
}