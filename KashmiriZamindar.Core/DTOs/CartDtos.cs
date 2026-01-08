namespace KashmiriZamindar.Core.Dtos
{
    public class CartDto
    {
        public string SessionId { get; set; }
        public Guid? UserGuid { get; set; }
        public List<CartItemDto> Items { get; set; } = new();
        public decimal Subtotal { get; set; }
        public int TotalItems { get; set; }
    }

    public class CartItemDto
    {
        public int CartItemId { get; set; }
        public Guid ProductGuid { get; set; }
        public string ProductName { get; set; }
        public string ProductCategory { get; set; }
        public string ProductUnit { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal ItemTotal { get; set; }
    }

    public class AddToCartDto
    {
        public string SessionId { get; set; }      // For guests
        public Guid? UserGuid { get; set; }        // For logged-in users
        public Guid ProductGuid { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public class UpdateCartItemDto
    {
        public int Quantity { get; set; }
    }

    public class MergeCartDto
    {
        public string GuestSessionId { get; set; }
        public Guid UserGuid { get; set; }
    }
}