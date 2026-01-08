namespace KashmiriZamindar.Core.Dtos
{
    public class AdminLoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class AdminAuthResponseDto
    {
        public Guid AdminGuid { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }

    public class DashboardStatsDto
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalProducts { get; set; }
        public int PendingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public decimal TodayRevenue { get; set; }
        public int TodayOrders { get; set; }
    }

    public class RecentOrderDto
    {
        public Guid OrderGuid { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime OrderDate { get; set; }
    }

    public class TopProductDto
    {
        public Guid ProductGuid { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public int TotalSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public string ImageUrl { get; set; }
    }

    public class SalesChartDto
    {
        public string Date { get; set; }
        public decimal Revenue { get; set; }
        public int Orders { get; set; }
    }

    public class OrderManagementDto
    {
        public Guid OrderGuid { get; set; }
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentType { get; set; }
        public DateTime OrderDate { get; set; }
        public string ShippingAddress { get; set; }
        public List<OrderItemDetailDto> Items { get; set; }
    }

    public class OrderItemDetailDto
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }

    public class UpdateOrderStatusDto
    {
        public string OrderStatus { get; set; }
        public string PaymentStatus { get; set; }
    }
}