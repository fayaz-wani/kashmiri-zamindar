// ============================================
// AdminService.cs
// ============================================
using API.Core.Dtos;
using KashmiriZamindar.Core.Dtos;
using KashmiriZamindar.Core.Helpers;
using KashmiriZamindar.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KashmiriZamindar.Core.Services
{
    public partial class AdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly string _jwtSecret;
        
        public AdminService(IAdminRepository adminRepository, IConfiguration configuration, IEmailService emailService)
        {
            _adminRepository = adminRepository;
            _configuration = configuration;
            _emailService = emailService;
            _jwtSecret = _configuration["JwtSettings:SecretKey"]
                ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
        }

        // ============================================
        // LOGIN
        // ============================================
        public async Task<AdminAuthResponseDto> LoginAsync(AdminLoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentException("Email is required");

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new ArgumentException("Password is required");

            var admin = await _adminRepository.GetAdminByEmailAsync(dto.Email);

            if (admin == null)
                throw new UnauthorizedAccessException("Invalid email or password");

            if (!admin.IsActive)
                throw new UnauthorizedAccessException("Account is deactivated");

            bool isPasswordValid = PasswordHelper.VerifyPassword(dto.Password, admin.PasswordHash);

            if (!isPasswordValid)
                throw new UnauthorizedAccessException("Invalid email or password");

            var user = new Entities.User
            {
                UserGuid = admin.AdminGuid,
                Email = admin.Email,
                FirstName = admin.FullName,
                LastName = ""
            };

            string token = JwtHelper.GenerateToken(user, _jwtSecret);

            return new AdminAuthResponseDto
            {
                AdminGuid = admin.AdminGuid,
                Email = admin.Email,
                FullName = admin.FullName,
                Role = admin.Role,
                Token = token
            };
        }

        // ============================================
        // DASHBOARD
        // ============================================
        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            return await _adminRepository.GetDashboardStatsAsync();
        }

        public async Task<List<RecentOrderDto>> GetRecentOrdersAsync(int top = 10)
        {
            return await _adminRepository.GetRecentOrdersAsync(top);
        }

        public async Task<List<TopProductDto>> GetTopProductsAsync(int top = 5)
        {
            return await _adminRepository.GetTopProductsAsync(top);
        }

        public async Task<List<SalesChartDto>> GetSalesChartDataAsync(int days = 7)
        {
            return await _adminRepository.GetSalesChartDataAsync(days);
        }

        // ============================================
        // ORDERS
        // ============================================
        public async Task<List<OrderManagementDto>> GetAllOrdersAsync(int page, int pageSize, string status)
        {
            return await _adminRepository.GetAllOrdersAsync(page, pageSize, status);
        }

        public async Task<OrderManagementDto?> GetOrderDetailsAsync(Guid orderGuid)
        {
            return await _adminRepository.GetOrderDetailsAsync(orderGuid);
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid orderGuid, UpdateOrderStatusDto dto)
        {
            return await _adminRepository.UpdateOrderStatusAsync(orderGuid, dto.OrderStatus, dto.PaymentStatus);
        }
    }

    // ============================================
    // PRODUCT METHODS (Partial)
    // ============================================
    public partial class AdminService
    {
        public async Task<List<AdminProductDto>> GetAllProductsAsync()
        {
            return await _adminRepository.GetAllProductsForAdminAsync();
        }

        public async Task<AdminProductDto?> GetProductDetailsAsync(Guid productGuid)
        {
            return await _adminRepository.GetProductWithImagesAsync(productGuid);
        }

        public async Task<Guid> CreateProductAsync(CreateProductDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Product name is required");

            if (dto.Price <= 0)
                throw new ArgumentException("Price must be greater than 0");

            return await _adminRepository.CreateProductAsync(dto);
        }

        public async Task<bool> UpdateProductAsync(Guid productGuid, UpdateProductDto dto)
        {
            return await _adminRepository.UpdateProductAsync(productGuid, dto);
        }

        public async Task<bool> DeleteProductImageAsync(int imageId)
        {
            return await _adminRepository.DeleteProductImageAsync(imageId);
        }

        public async Task<bool> AddProductImageAsync(Guid productGuid, ProductImageUploadDto dto)
        {
            return await _adminRepository.AddProductImageAsync(productGuid, dto);
        }

        public async Task<bool> ToggleProductStatusAsync(Guid productGuid)
        {
            return await _adminRepository.ToggleProductStatusAsync(productGuid);
        }

        // ============================================
// INVENTORY & ADVANCED ORDER MANAGEMENT
// ============================================


            // ================================
            // INVENTORY
            // ================================

            public async Task<List<InventoryItemDto>> GetInventoryOverviewAsync()
            {
                return await _adminRepository.GetInventoryOverviewAsync();
            }

            public async Task<bool> UpdateStockQuantityAsync(Guid productGuid, UpdateStockDto dto)
            {
                if (dto.QuantityChange == 0)
                    throw new ArgumentException("Quantity change cannot be zero");

                if (string.IsNullOrWhiteSpace(dto.ChangeType))
                    throw new ArgumentException("Change type is required");

                return await _adminRepository.UpdateStockQuantityAsync(productGuid, dto);
            }

            public async Task<List<InventoryHistoryDto>> GetInventoryHistoryAsync(Guid? productGuid, int days = 30)
            {
                return await _adminRepository.GetInventoryHistoryAsync(productGuid, days);
            }

            public async Task<List<LowStockAlertDto>> GetLowStockProductsAsync()
            {
                return await _adminRepository.GetLowStockProductsAsync();
            }

            // ================================
            // ADVANCED ORDER MANAGEMENT
            // ================================

            public async Task<List<OrderDetailDto>> GetOrdersAdvancedAsync(
                int page,
                int pageSize,
                string orderStatus,
                string paymentStatus,
                string paymentType,
                string searchTerm,
                DateTime? fromDate,
                DateTime? toDate)
            {
                return await _adminRepository.GetOrdersAdvancedAsync(
                    page,
                    pageSize,
                    orderStatus,
                    paymentStatus,
                    paymentType,
                    searchTerm,
                    fromDate,
                    toDate
                );
            }

            public async Task<OrderDetailDto?> GetOrderDetailsCompleteAsync(Guid orderGuid)
            {
                return await _adminRepository.GetOrderDetailsCompleteAsync(orderGuid);
            }

            public async Task<OrderStatisticsDto> GetOrderStatisticsAsync(int days = 30)
            {
                return await _adminRepository.GetOrderStatisticsAsync(days);
            }

            public async Task<int> BulkUpdateOrderStatusAsync(BulkUpdateStatusDto dto)
            {
                if (dto.OrderGuids == null || dto.OrderGuids.Count == 0)
                    throw new ArgumentException("Order GUID list cannot be empty");

                if (string.IsNullOrWhiteSpace(dto.OrderStatus))
                    throw new ArgumentException("Order status is required");

                return await _adminRepository.BulkUpdateOrderStatusAsync(
                    dto.OrderGuids,
                    dto.OrderStatus,
                    dto.PaymentStatus
                );
            }

        // ============================================
        // CUSTOMER MANAGEMENT (Partial)
        // ============================================

            public async Task<CustomerListResponse> GetCustomersAdvancedAsync(
                int page,
                int pageSize,
                string searchTerm = null,
                string customerStatus = null,
                DateTime? fromDate = null,
                DateTime? toDate = null,
                int? minOrders = null,
                int? maxOrders = null,
                decimal? minSpending = null,
                decimal? maxSpending = null)
            {
                return await _adminRepository.GetCustomersAdvancedAsync(
                    page,
                    pageSize,
                    searchTerm,
                    customerStatus,
                    fromDate,
                    toDate,
                    minOrders,
                    maxOrders,
                    minSpending,
                    maxSpending
                );
            }

            public async Task<CustomerDetailDto?> GetCustomerDetailsCompleteAsync(Guid userGuid)
            {
                return await _adminRepository.GetCustomerDetailsCompleteAsync(userGuid);
            }

            public async Task<CustomerStatisticsDto> GetCustomerStatisticsAsync()
            {
                return await _adminRepository.GetCustomerStatisticsAsync();
            }

            public async Task<bool> ToggleCustomerStatusAsync(Guid userGuid)
            {
                return await _adminRepository.ToggleCustomerStatusAsync(userGuid);
            }

            public async Task<List<CustomerValueSegmentDto>> GetCustomerValueSegmentsAsync()
            {
                return await _adminRepository.GetCustomerValueSegmentsAsync();
            }

            public async Task<List<TopCustomerDto>> GetTopCustomersAsync(int topCount = 10, string orderBy = "Spending")
            {
                if (topCount <= 0)
                    topCount = 10;

                if (string.IsNullOrWhiteSpace(orderBy))
                    orderBy = "Spending";

                return await _adminRepository.GetTopCustomersAsync(topCount, orderBy);
            }

            public async Task<List<CustomerDto>> ExportCustomerDataAsync()
            {
                return await _adminRepository.ExportCustomerDataAsync();
            }

        // Add these methods to your existing AdminService partial class

        public async Task<SalesAnalyticsDto> GetSalesAnalyticsAsync(int days = 30)
        {
            return await _adminRepository.GetSalesAnalyticsAsync(days);
        }

        public async Task<ProductAnalyticsDto> GetProductAnalyticsAsync(int days = 30)
        {
            return await _adminRepository.GetProductAnalyticsAsync(days);
        }

        public async Task<CustomerAnalyticsDto> GetCustomerAnalyticsAsync(int days = 30)
        {
            return await _adminRepository.GetCustomerAnalyticsAsync(days);
        }

        public async Task<RevenueReportDto> GetRevenueReportsAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Start date must be before end date");

            return await _adminRepository.GetRevenueReportsAsync(startDate, endDate);
        }

        public async Task<InventoryReportDto> GetInventoryReportsAsync()
        {
            return await _adminRepository.GetInventoryReportsAsync();
        }

        public async Task<OrderFulfillmentReportDto> GetOrderFulfillmentReportAsync(int days = 30)
        {
            return await _adminRepository.GetOrderFulfillmentReportAsync(days);
        }

        public async Task<ComparisonReportDto> GetComparisonReportAsync(
            DateTime period1Start, DateTime period1End,
            DateTime period2Start, DateTime period2End)
        {
            if (period1Start > period1End)
                throw new ArgumentException("Period 1 start date must be before end date");

            if (period2Start > period2End)
                throw new ArgumentException("Period 2 start date must be before end date");

            return await _adminRepository.GetComparisonReportAsync(
                period1Start, period1End, period2Start, period2End);
        }

        public async Task<ExecutiveSummaryDto> GetExecutiveSummaryAsync()
        {
            return await _adminRepository.GetExecutiveSummaryAsync();
        }

        // Add these methods to your existing AdminService class



            // =====================================================
            // EMAIL NOTIFICATION METHODS
            // =====================================================

            public async Task<EmailNotificationListResponse> GetEmailNotificationHistoryAsync(
                int days,
                string status,
                string notificationType,
                int page,
                int pageSize)
            {
                return await _adminRepository.GetEmailNotificationHistoryAsync(
                    days, status, notificationType, page, pageSize);
            }

            public async Task<EmailStatisticsDto> GetEmailNotificationStatisticsAsync(int days)
            {
                return await _adminRepository.GetEmailNotificationStatisticsAsync(days);
            }

            public async Task<int> CreateEmailNotificationAsync(CreateEmailNotificationDto dto)
            {
                // Validate email
                if (string.IsNullOrEmpty(dto.RecipientEmail))
                {
                    throw new ArgumentException("Recipient email is required");
                }

                if (string.IsNullOrEmpty(dto.Subject))
                {
                    throw new ArgumentException("Subject is required");
                }

                if (string.IsNullOrEmpty(dto.Body))
                {
                    throw new ArgumentException("Body is required");
                }

                return await _adminRepository.CreateEmailNotificationAsync(dto);
            }

            public async Task ProcessPendingEmailsAsync()
            {
                await _emailService.ProcessPendingEmailsAsync();
            }

            public async Task<List<EmailTemplateDto>> GetEmailTemplatesAsync()
            {
                return await _adminRepository.GetEmailTemplatesAsync();
            }

            public async Task<bool> SaveEmailTemplateAsync(SaveEmailTemplateDto dto)
            {
                if (string.IsNullOrEmpty(dto.TemplateName))
                {
                    throw new ArgumentException("Template name is required");
                }

                if (string.IsNullOrEmpty(dto.Subject))
                {
                    throw new ArgumentException("Subject is required");
                }

                if (string.IsNullOrEmpty(dto.BodyTemplate))
                {
                    throw new ArgumentException("Body template is required");
                }

                return await _adminRepository.SaveEmailTemplateAsync(dto);
            }

            public async Task<bool> SendTestEmailAsync(SendTestEmailDto dto)
            {
                if (string.IsNullOrEmpty(dto.RecipientEmail))
                {
                    throw new ArgumentException("Recipient email is required");
                }

                if (string.IsNullOrEmpty(dto.TemplateName))
                {
                    throw new ArgumentException("Template name is required");
                }

                return await _emailService.SendTemplateEmailAsync(
                    dto.RecipientEmail,
                    dto.TemplateName,
                    dto.TemplateData
                );
            }

            public async Task<bool> SendOrderConfirmationEmailAsync(int orderId)
            {
                return await _adminRepository.SendOrderConfirmationEmailAsync(orderId);
            }
        }
    }

  
 
  
   
    





