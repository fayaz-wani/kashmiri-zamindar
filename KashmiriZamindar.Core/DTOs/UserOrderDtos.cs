// Core/Dtos/UserOrderDtos.cs

namespace KashmiriZamindar.Core.Dtos
{
    public class UserOrderDto
    {
        public Guid OrderGuid { get; set; }
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string PaymentType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public int ItemCount { get; set; }
    }

    public class UserOrderDetailDto
    {
        public Guid OrderGuid { get; set; }
        public int OrderId { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string BillingAddress { get; set; } = string.Empty;
        public string PaymentType { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string OrderStatus { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<UserOrderItemDto> Items { get; set; } = new();
    }

    public class UserOrderItemDto
    {
        public int OrderItemId { get; set; }
        public Guid ProductGuid { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Subtotal { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }

    public class UserOrdersResponse
    {
        public List<UserOrderDto> Orders { get; set; } = new();
        public int TotalOrders { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}