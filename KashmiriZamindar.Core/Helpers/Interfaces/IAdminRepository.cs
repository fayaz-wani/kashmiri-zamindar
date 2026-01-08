using API.Core.Dtos;
using KashmiriZamindar.Core.Dtos;
using KashmiriZamindar.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KashmiriZamindar.Core.Interfaces
{
    public interface IAdminRepository
    {
        // =========================
        // ADMIN LOGIN
        // =========================
        Task<AdminUser?> GetAdminByEmailAsync(string email);

        // =========================
        // DASHBOARD
        // =========================
        Task<DashboardStatsDto> GetDashboardStatsAsync();
        Task<List<RecentOrderDto>> GetRecentOrdersAsync(int top = 10);
        Task<List<TopProductDto>> GetTopProductsAsync(int top = 5);
        Task<List<SalesChartDto>> GetSalesChartDataAsync(int days = 7);

        // =========================
        // ORDERS
        // =========================
        Task<List<OrderManagementDto>> GetAllOrdersAsync(int pageNumber, int pageSize, string status);
        Task<OrderManagementDto?> GetOrderDetailsAsync(Guid orderGuid);
        Task<bool> UpdateOrderStatusAsync(Guid orderGuid, string orderStatus, string paymentStatus);

        // =========================
        // PRODUCTS
        // =========================
        Task<List<AdminProductDto>> GetAllProductsForAdminAsync();
        Task<AdminProductDto?> GetProductWithImagesAsync(Guid productGuid);
        Task<Guid> CreateProductAsync(CreateProductDto dto);
        Task<bool> UpdateProductAsync(Guid productGuid, UpdateProductDto dto);
        Task<bool> DeleteProductImageAsync(int imageId);
        Task<bool> AddProductImageAsync(Guid productGuid, ProductImageUploadDto dto);
        Task<bool> ToggleProductStatusAsync(Guid productGuid);

        Task<List<InventoryItemDto>> GetInventoryOverviewAsync();
        Task<bool> UpdateStockQuantityAsync(Guid productGuid, UpdateStockDto dto);
        Task<List<InventoryHistoryDto>> GetInventoryHistoryAsync(Guid? productGuid, int days);
        Task<List<LowStockAlertDto>> GetLowStockProductsAsync();

        Task<List<OrderDetailDto>> GetOrdersAdvancedAsync(
    int page, int pageSize, string orderStatus, string paymentStatus,
    string paymentType, string searchTerm, DateTime? fromDate, DateTime? toDate);
        Task<OrderDetailDto?> GetOrderDetailsCompleteAsync(Guid orderGuid);
        Task<OrderStatisticsDto> GetOrderStatisticsAsync(int days);
        Task<int> BulkUpdateOrderStatusAsync(List<string> orderGuids, string orderStatus, string paymentStatus);

        // CUSTOMER MANAGEMENT
        // =========================
        Task<CustomerListResponse> GetCustomersAdvancedAsync(
            int page, int pageSize, string searchTerm, string customerStatus,
            DateTime? fromDate, DateTime? toDate, int? minOrders, int? maxOrders,
            decimal? minSpending, decimal? maxSpending);
        Task<CustomerDetailDto?> GetCustomerDetailsCompleteAsync(Guid userGuid);
        Task<CustomerStatisticsDto> GetCustomerStatisticsAsync();
        Task<bool> ToggleCustomerStatusAsync(Guid userGuid);
        Task<List<CustomerValueSegmentDto>> GetCustomerValueSegmentsAsync();
        Task<List<TopCustomerDto>> GetTopCustomersAsync(int topCount, string orderBy);
        Task<List<CustomerDto>> ExportCustomerDataAsync();

        // Analytics report

        Task<SalesAnalyticsDto> GetSalesAnalyticsAsync(int days);
        Task<ProductAnalyticsDto> GetProductAnalyticsAsync(int days);
        Task<CustomerAnalyticsDto> GetCustomerAnalyticsAsync(int days);
        Task<RevenueReportDto> GetRevenueReportsAsync(DateTime startDate, DateTime endDate);
        Task<InventoryReportDto> GetInventoryReportsAsync();
        Task<OrderFulfillmentReportDto> GetOrderFulfillmentReportAsync(int days);
        Task<ComparisonReportDto> GetComparisonReportAsync(
           DateTime period1Start, DateTime period1End,
           DateTime period2Start, DateTime period2End);
        Task<ExecutiveSummaryDto> GetExecutiveSummaryAsync();

        Task<EmailNotificationListResponse> GetEmailNotificationHistoryAsync(
    int days, string status, string notificationType, int page, int pageSize);
        Task<EmailStatisticsDto> GetEmailNotificationStatisticsAsync(int days);
        Task<int> CreateEmailNotificationAsync(CreateEmailNotificationDto dto);
        //Task ProcessPendingEmailsAsync();
        Task<List<EmailTemplateDto>> GetEmailTemplatesAsync();
        Task<bool> SaveEmailTemplateAsync(SaveEmailTemplateDto dto);
        //Task<bool> SendTestEmailAsync(SendTestEmailDto dto);
        Task<bool> SendOrderConfirmationEmailAsync(int orderId);
        // EMAIL (REQUIRED BY EmailService)

        Task<EmailTemplateDto?> GetEmailTemplateByNameAsync(string templateName);

        Task<List<EmailNotificationDto>> GetPendingEmailNotificationsAsync(int top);

        Task<bool> UpdateEmailNotificationStatusAsync(
            Guid notificationGuid,
            string status,
            string? failureReason
        );
        // IAdminRepository.cs - Add this method signature
        

    }
}
