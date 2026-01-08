namespace API.Core.Dtos
{
    // Customer List DTO
    public class CustomerDto
    {
        public Guid UserGuid { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal AverageOrderValue { get; set; }
        public DateTime? LastOrderDate { get; set; }
        public int OrdersLast30Days { get; set; }
        public string CustomerStatus { get; set; } = string.Empty; // New, Active, Inactive, Churned
        public string? City { get; set; }
        public string? State { get; set; }
    }

    // Customer Detail DTO
    public class CustomerDetailDto
    {
        public Guid UserGuid { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; }
        public string Role { get; set; } = string.Empty;
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal AverageOrderValue { get; set; }
        public DateTime? LastOrderDate { get; set; }
        public int OrdersLast30Days { get; set; }
        public int OrdersLast90Days { get; set; }
        public string CustomerStatus { get; set; } = string.Empty;

        // Related data
        public List<CustomerAddressDto> Addresses { get; set; } = new();
        public List<CustomerOrderDto> RecentOrders { get; set; } = new();
        public List<CustomerCategoryDto> FavoriteCategories { get; set; } = new();
        public List<CustomerSpendingTrendDto> SpendingTrend { get; set; } = new();
    }

    public class CustomerAddressDto
    {
        public Guid AddressGuid { get; set; }
        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CustomerOrderDto
    {
        public Guid OrderGuid { get; set; }
        public string OrderId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string PaymentType { get; set; } = string.Empty;
        public int ItemCount { get; set; }
    }

    public class CustomerCategoryDto
    {
        public string Category { get; set; } = string.Empty;
        public int PurchaseCount { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalSpent { get; set; }
    }

    public class CustomerSpendingTrendDto
    {
        public string Month { get; set; } = string.Empty;
        public int OrderCount { get; set; }
        public decimal TotalSpent { get; set; }
    }

    // Customer Statistics DTO
    public class CustomerStatisticsDto
    {
        public int TotalCustomers { get; set; }
        public int NewCustomers { get; set; }
        public int ActiveCustomers { get; set; }
        public int InactiveCustomers { get; set; }
        public int ChurnedCustomers { get; set; }
        public int CustomersWithOrders { get; set; }
        public int CustomersWithoutOrders { get; set; }
        public decimal AverageOrdersPerCustomer { get; set; }
        public decimal AverageLifetimeValue { get; set; }
        public int CustomersLast7Days { get; set; }
        public int CustomersLast30Days { get; set; }
    }

    // Customer Value Segment DTO
    public class CustomerValueSegmentDto
    {
        public string Segment { get; set; } = string.Empty;
        public int CustomerCount { get; set; }
        public decimal AverageValue { get; set; }
        public decimal TotalValue { get; set; }
    }

    // Top Customer DTO
    public class TopCustomerDto
    {
        public Guid UserGuid { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal AverageOrderValue { get; set; }
        public DateTime? LastOrderDate { get; set; }
    }

    // Customer List Response
    public class CustomerListResponse
    {
        public List<CustomerDto> Customers { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}