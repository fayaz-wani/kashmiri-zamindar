namespace KashmiriZamindar.Core.Entities
{
    public class Cart
    {
        public int CartId { get; set; }

        // For Guest Users (browser-based)
        public string SessionId { get; set; }  // Generated session ID for guests

        // For Logged-in Users (persistent)
        public Guid? UserGuid { get; set; }  // Nullable - null for guests

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation
        public User User { get; set; }
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }

    public class CartItem
    {
        public int CartItemId { get; set; }
        public int CartId { get; set; }
        public Guid ProductGuid { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtAdd { get; set; }
        public DateTime AddedAt { get; set; }

        // Navigation
        public Cart Cart { get; set; }
        public Product Product { get; set; }
    }
}