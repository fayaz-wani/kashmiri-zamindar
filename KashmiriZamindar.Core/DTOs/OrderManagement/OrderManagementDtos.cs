using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KashmiriZamindar.Core.Dtos
{
    // Order Management DTOs
    public class OrderDetailDto
    {
        public Guid OrderGuid { get; set; }
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Shipping { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentType { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ShippingAddress { get; set; }
        public string BillingAddress { get; set; }
        public string? DeliveryInstructions { get; set; }
        public string? OrderNotes { get; set; }
        public int ItemCount { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderItemDto
    {
        public int OrderItemId { get; set; }
        public Guid ProductGuid { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Subtotal { get; set; }
        public string ImageUrl { get; set; }
        public int StockQuantity { get; set; }
    }

    public class OrderStatisticsDto
    {
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int ProcessingOrders { get; set; }
        public int ShippedOrders { get; set; }
        public int DeliveredOrders { get; set; }
        public int CancelledOrders { get; set; }
        public int PaidOrders { get; set; }
        public int UnpaidOrders { get; set; }
        public int CODOrders { get; set; }
        public int OnlineOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
    }

    public class BulkUpdateStatusDto
    {
        public List<string> OrderGuids { get; set; } = new();
        public string OrderStatus { get; set; }
        public string PaymentStatus { get; set; }
    }
}

