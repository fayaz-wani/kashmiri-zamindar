using KashmiriZamindar.Core.Dtos;

namespace API.Core.Dtos
{
    // Sales Analytics
    public class SalesAnalyticsDto
    {
        public List<DailySalesDto> DailySales { get; set; } = new();
        public SalesSummaryDto Summary { get; set; } = new();
        public List<HourlyPatternDto> HourlyPattern { get; set; } = new();
        public List<DayOfWeekPatternDto> DayOfWeekPattern { get; set; } = new();
    }

    public class DailySalesDto
    {
        public DateTime Date { get; set; }
        public int OrderCount { get; set; }
        public decimal Revenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int UniqueCustomers { get; set; }
    }

    public class SalesSummaryDto
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int TotalCustomers { get; set; }
        public decimal DeliveredRevenue { get; set; }
        public decimal CancelledRevenue { get; set; }
    }

    public class HourlyPatternDto
    {
        public int Hour { get; set; }
        public int OrderCount { get; set; }
        public decimal Revenue { get; set; }
    }

    public class DayOfWeekPatternDto
    {
        public string DayOfWeek { get; set; } = string.Empty;
        public int DayNumber { get; set; }
        public int OrderCount { get; set; }
        public decimal Revenue { get; set; }
    }

    // Product Analytics
    public class ProductAnalyticsDto
    {
        public List<TopProductDto> TopProducts { get; set; } = new();
        public List<CategoryPerformanceDto> CategoryPerformance { get; set; } = new();
        public List<LowPerformerDto> LowPerformers { get; set; } = new();
    }

    public class CategoryPerformanceDto
    {
        public string Category { get; set; } = string.Empty;
        public int ProductCount { get; set; }
        public int TotalSales { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AveragePrice { get; set; }
    }

    public class LowPerformerDto
    {
        public Guid ProductGuid { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int TimesSold { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    // Customer Analytics
    public class CustomerAnalyticsDto
    {
        public List<CustomerAcquisitionDto> CustomerAcquisition { get; set; } = new();
        public CustomerRetentionDto Retention { get; set; } = new();
        public List<CustomerValueDistributionDto> ValueDistribution { get; set; } = new();
        public List<GeographicDistributionDto> GeographicDistribution { get; set; } = new();
    }

    public class CustomerAcquisitionDto
    {
        public DateTime Date { get; set; }
        public int NewCustomers { get; set; }
    }

    public class CustomerRetentionDto
    {
        public int ActiveCustomers { get; set; }
        public int ReturningCustomers { get; set; }
        public int NewCustomersWithOrders { get; set; }
    }

    public class CustomerValueDistributionDto
    {
        public string ValueSegment { get; set; } = string.Empty;
        public int CustomerCount { get; set; }
        public decimal AverageValue { get; set; }
        public decimal TotalValue { get; set; }
    }

    public class GeographicDistributionDto
    {
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public int CustomerCount { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    // Revenue Reports
    public class RevenueReportDto
    {
        public List<DailyRevenueDto> DailyRevenue { get; set; } = new();
        public List<PaymentMethodBreakdownDto> PaymentMethods { get; set; } = new();
        public List<CategoryRevenueDto> CategoryRevenue { get; set; } = new();
        public RevenueSummaryDto Summary { get; set; } = new();
    }

    public class DailyRevenueDto
    {
        public DateTime Date { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Shipping { get; set; }
        public int OrderCount { get; set; }
    }

    public class PaymentMethodBreakdownDto
    {
        public string PaymentType { get; set; } = string.Empty;
        public int OrderCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
    }

    public class CategoryRevenueDto
    {
        public string Category { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
        public int TotalQuantity { get; set; }
    }

    public class RevenueSummaryDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalSubtotal { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalShipping { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int UniqueCustomers { get; set; }
    }

    // Inventory Reports
    public class InventoryReportDto
    {
        public List<StockStatusDto> StockStatus { get; set; } = new();
        public List<StockMovementDto> StockMovement { get; set; } = new();
        public StockAlertsSummaryDto AlertsSummary { get; set; } = new();
    }

    public class StockStatusDto
    {
        public Guid ProductGuid { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int LowStockThreshold { get; set; }
        public string StockStatus { get; set; } = string.Empty;
        public decimal StockValue { get; set; }
    }

    public class StockMovementDto
    {
        public Guid ProductGuid { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ChangeType { get; set; } = string.Empty;
        public int TotalChange { get; set; }
        public int TransactionCount { get; set; }
    }

    public class StockAlertsSummaryDto
    {
        public int OutOfStockCount { get; set; }
        public int LowStockCount { get; set; }
        public int InStockCount { get; set; }
        public decimal TotalStockValue { get; set; }
    }

    // Order Fulfillment Report
    public class OrderFulfillmentReportDto
    {
        public List<OrderStatusBreakdownDto> StatusBreakdown { get; set; } = new();
        public List<DeliveryPerformanceDto> DeliveryPerformance { get; set; } = new();
        public FulfillmentTimeDto FulfillmentTime { get; set; } = new();
    }

    public class OrderStatusBreakdownDto
    {
        public string OrderStatus { get; set; } = string.Empty;
        public int OrderCount { get; set; }
        public decimal Revenue { get; set; }
        public decimal AverageDays { get; set; }
    }

    public class DeliveryPerformanceDto
    {
        public DateTime Date { get; set; }
        public int DeliveredOrders { get; set; }
        public int CancelledOrders { get; set; }
        public int PendingOrders { get; set; }
    }

    public class FulfillmentTimeDto
    {
        public decimal AverageFulfillmentDays { get; set; }
        public decimal MinFulfillmentDays { get; set; }
        public decimal MaxFulfillmentDays { get; set; }
    }

    // Comparison Report
    public class ComparisonReportDto
    {
        public PeriodMetricsDto Period1 { get; set; } = new();
        public PeriodMetricsDto Period2 { get; set; } = new();
    }

    public class PeriodMetricsDto
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int UniqueCustomers { get; set; }
        public int TotalItemsSold { get; set; }
    }

    // Executive Summary
    public class ExecutiveSummaryDto
    {
        public OverallMetricsDto OverallMetrics { get; set; } = new();
        public List<TopProductSummaryDto> TopProducts { get; set; } = new();
        public GrowthMetricsDto GrowthMetrics { get; set; } = new();
    }

    public class OverallMetricsDto
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int TotalCustomers { get; set; }
        public int RegisteredUsers { get; set; }
        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public int OrdersLast30Days { get; set; }
        public decimal RevenueLast30Days { get; set; }
        public int NewCustomersLast30Days { get; set; }
    }

    public class TopProductSummaryDto
    {
            public string ProductName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int TotalSold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class GrowthMetricsDto
    {
        public decimal RecentRevenue { get; set; }
        public decimal PreviousRevenue { get; set; }
        public int RecentOrders { get; set; }
        public int PreviousOrders { get; set; }
        public decimal RevenueGrowth { get; set; }
        public decimal OrderGrowth { get; set; }
    }
}