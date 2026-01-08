// ============================================
// AdminController.cs
// ============================================
using API.Core.Dtos;
using KashmiriZamindar.Core.Dtos;
using KashmiriZamindar.Core.Helpers;
using KashmiriZamindar.Core.Interfaces;
using KashmiriZamindar.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace KashmiriZamindar.API.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly AdminService _adminService;

        public AdminController(AdminService adminService)
        {
            _adminService = adminService;
        }

        // =====================================================
        // AUTH
        // =====================================================

        // POST: api/admin/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AdminLoginDto dto)
        {
            try
            {
                var response = await _adminService.LoginAsync(dto);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred during login",
                    error = ex.Message
                });
            }
        }

        // =====================================================
        // DASHBOARD
        // =====================================================

        // GET: api/admin/dashboard/stats
        [HttpGet("dashboard/stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var stats = await _adminService.GetDashboardStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching dashboard stats",
                    error = ex.Message
                });
            }
        }

        // GET: api/admin/dashboard/recent-orders
        [HttpGet("dashboard/recent-orders")]
        public async Task<IActionResult> GetRecentOrders([FromQuery] int top = 10)
        {
            try
            {
                var orders = await _adminService.GetRecentOrdersAsync(top);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching recent orders",
                    error = ex.Message
                });
            }
        }

        // GET: api/admin/dashboard/top-products
        [HttpGet("dashboard/top-products")]
        public async Task<IActionResult> GetTopProducts([FromQuery] int top = 5)
        {
            try
            {
                var products = await _adminService.GetTopProductsAsync(top);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching top products",
                    error = ex.Message
                });
            }
        }

        // GET: api/admin/dashboard/sales-chart
        [HttpGet("dashboard/sales-chart")]
        public async Task<IActionResult> GetSalesChart([FromQuery] int days = 7)
        {
            try
            {
                var data = await _adminService.GetSalesChartDataAsync(days);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching sales chart",
                    error = ex.Message
                });
            }
        }

        // =====================================================
        // ORDERS
        // =====================================================

        // GET: api/admin/orders
        [HttpGet("orders")]
        public async Task<IActionResult> GetAllOrders(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string status = null)
        {
            try
            {
                var orders = await _adminService.GetAllOrdersAsync(page, pageSize, status);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching orders",
                    error = ex.Message
                });
            }
        }

        // GET: api/admin/orders/{orderGuid}
        [HttpGet("orders/{orderGuid}")]
        public async Task<IActionResult> GetOrderDetails(Guid orderGuid)
        {
            try
            {
                var order = await _adminService.GetOrderDetailsAsync(orderGuid);
                if (order == null)
                    return NotFound(new { message = "Order not found" });

                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching order details",
                    error = ex.Message
                });
            }
        }

        // PUT: api/admin/orders/{orderGuid}/status
        [HttpPut("orders/{orderGuid}/status")]
        public async Task<IActionResult> UpdateOrderStatus(
            Guid orderGuid,
            [FromBody] UpdateOrderStatusDto dto)
        {
            try
            {
                var success = await _adminService.UpdateOrderStatusAsync(orderGuid, dto);
                if (!success)
                    return NotFound(new { message = "Order not found" });

                return Ok(new { message = "Order status updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error updating order status",
                    error = ex.Message
                });
            }
        }

        // =====================================================
        // PRODUCTS (NEWLY ADDED)
        // =====================================================

        // GET: api/admin/products
        [HttpGet("products")]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await _adminService.GetAllProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching products",
                    error = ex.Message
                });
            }
        }

        // GET: api/admin/products/{productGuid}
        [HttpGet("products/{productGuid}")]
        public async Task<IActionResult> GetProductDetails(Guid productGuid)
        {
            try
            {
                var product = await _adminService.GetProductDetailsAsync(productGuid);
                if (product == null)
                    return NotFound(new { message = "Product not found" });

                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching product",
                    error = ex.Message
                });
            }
        }

        // POST: api/admin/products
        [HttpPost("products")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
        {
            try
            {
                var productGuid = await _adminService.CreateProductAsync(dto);
                return Ok(new
                {
                    productGuid,
                    message = "Product created successfully"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error creating product",
                    error = ex.Message
                });
            }
        }

        // PUT: api/admin/products/{productGuid}
        [HttpPut("products/{productGuid}")]
        public async Task<IActionResult> UpdateProduct(
            Guid productGuid,
            [FromBody] UpdateProductDto dto)
        {
            try
            {
                var success = await _adminService.UpdateProductAsync(productGuid, dto);
                if (!success)
                    return NotFound(new { message = "Product not found" });

                return Ok(new { message = "Product updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error updating product",
                    error = ex.Message
                });
            }
        }

        // POST: api/admin/products/{productGuid}/images
        [HttpPost("products/{productGuid}/images")]
        public async Task<IActionResult> AddProductImage(
            Guid productGuid,
            [FromBody] ProductImageUploadDto dto)
        {
            try
            {
                var success = await _adminService.AddProductImageAsync(productGuid, dto);
                if (!success)
                    return BadRequest(new { message = "Failed to add image" });

                return Ok(new { message = "Image added successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error adding image",
                    error = ex.Message
                });
            }
        }

        // DELETE: api/admin/products/images/{imageId}
        [HttpDelete("products/images/{imageId}")]
        public async Task<IActionResult> DeleteProductImage(int imageId)
        {
            try
            {
                var success = await _adminService.DeleteProductImageAsync(imageId);
                if (!success)
                    return NotFound(new { message = "Image not found" });

                return Ok(new { message = "Image deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error deleting image",
                    error = ex.Message
                });
            }
        }

        // PUT: api/admin/products/{productGuid}/toggle-status
        [HttpPut("products/{productGuid}/toggle-status")]
        public async Task<IActionResult> ToggleProductStatus(Guid productGuid)
        {
            try
            {
                await _adminService.ToggleProductStatusAsync(productGuid);
                return Ok(new { message = "Product status toggled successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error toggling product status",
                    error = ex.Message
                });
            }
        }
        // =====================================================
        // INVENTORY
        // =====================================================

        // GET: api/admin/inventory
        [HttpGet("inventory")]
        public async Task<IActionResult> GetInventoryOverview()
        {
            try
            {
                var inventory = await _adminService.GetInventoryOverviewAsync();
                return Ok(inventory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching inventory",
                    error = ex.Message
                });
            }
        }

        // PUT: api/admin/inventory/{productGuid}/stock
        [HttpPut("inventory/{productGuid}/stock")]
        public async Task<IActionResult> UpdateStock(
            Guid productGuid,
            [FromBody] UpdateStockDto dto)
        {
            try
            {
                var success = await _adminService.UpdateStockQuantityAsync(productGuid, dto);
                if (!success)
                    return NotFound(new { message = "Product not found" });

                return Ok(new { message = "Stock updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error updating stock",
                    error = ex.Message
                });
            }
        }

        // GET: api/admin/inventory/{productGuid}/history
        [HttpGet("inventory/{productGuid}/history")]
        public async Task<IActionResult> GetInventoryHistory(
            Guid productGuid,
            [FromQuery] int days = 30)
        {
            try
            {
                var history = await _adminService.GetInventoryHistoryAsync(productGuid, days);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching inventory history",
                    error = ex.Message
                });
            }
        }

        // GET: api/admin/inventory/alerts
        [HttpGet("inventory/alerts")]
        public async Task<IActionResult> GetLowStockAlerts()
        {
            try
            {
                var alerts = await _adminService.GetLowStockProductsAsync();
                return Ok(alerts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching stock alerts",
                    error = ex.Message
                });
            }
        }

        // =====================================================
        // ADVANCED ORDERS
        // =====================================================

        // GET: api/admin/orders/advanced
        [HttpGet("orders/advanced")]
        public async Task<IActionResult> GetOrdersAdvanced(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string orderStatus = null,
            [FromQuery] string paymentStatus = null,
            [FromQuery] string paymentType = null,
            [FromQuery] string searchTerm = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var orders = await _adminService.GetOrdersAdvancedAsync(
                    page,
                    pageSize,
                    orderStatus,
                    paymentStatus,
                    paymentType,
                    searchTerm,
                    fromDate,
                    toDate
                );

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching advanced orders",
                    error = ex.Message
                });
            }
        }

        // GET: api/admin/orders/{orderGuid}/complete
        [HttpGet("orders/{orderGuid}/complete")]
        public async Task<IActionResult> GetOrderComplete(Guid orderGuid)
        {
            try
            {
                var order = await _adminService.GetOrderDetailsCompleteAsync(orderGuid);
                if (order == null)
                    return NotFound(new { message = "Order not found" });

                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching complete order details",
                    error = ex.Message
                });
            }
        }

        // GET: api/admin/orders/statistics
        [HttpGet("orders/statistics")]
        public async Task<IActionResult> GetOrderStatistics([FromQuery] int days = 30)
        {
            try
            {
                var stats = await _adminService.GetOrderStatisticsAsync(days);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching order statistics",
                    error = ex.Message
                });
            }
        }

        // PUT: api/admin/orders/bulk-update
        [HttpPut("orders/bulk-update")]
        public async Task<IActionResult> BulkUpdateOrders([FromBody] BulkUpdateStatusDto dto)
        {
            try
            {
                var updatedCount = await _adminService.BulkUpdateOrderStatusAsync(dto);

                return Ok(new
                {
                    updatedOrders = updatedCount,
                    message = "Orders updated successfully"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error updating orders",
                    error = ex.Message
                });
            }
        }
        // =====================================================
        // CUSTOMER MANAGEMENT
        // =====================================================

        // GET: api/admin/customers
        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string searchTerm = null,
            [FromQuery] string customerStatus = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int? minOrders = null,
            [FromQuery] int? maxOrders = null,
            [FromQuery] decimal? minSpending = null,
            [FromQuery] decimal? maxSpending = null)
        {
            try
            {
                var response = await _adminService.GetCustomersAdvancedAsync(
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

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching customers",
                    error = ex.Message
                });
            }
        }

        // GET: api/admin/customers/{userGuid}
        [HttpGet("customers/{userGuid}")]
        public async Task<IActionResult> GetCustomerDetails(Guid userGuid)
        {
            try
            {
                var customer = await _adminService.GetCustomerDetailsCompleteAsync(userGuid);
                if (customer == null)
                    return NotFound(new { message = "Customer not found" });

                return Ok(customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching customer details",
                    error = ex.Message
                });
            }
        }

        // GET: api/admin/customers/statistics
        [HttpGet("customers/statistics")]
        public async Task<IActionResult> GetCustomerStatistics()
        {
            try
            {
                var stats = await _adminService.GetCustomerStatisticsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching customer statistics",
                    error = ex.Message
                });
            }
        }

        // PUT: api/admin/customers/{userGuid}/toggle-status
        [HttpPut("customers/{userGuid}/toggle-status")]
        public async Task<IActionResult> ToggleCustomerStatus(Guid userGuid)
        {
            try
            {
                var success = await _adminService.ToggleCustomerStatusAsync(userGuid);
                if (!success)
                    return NotFound(new { message = "Customer not found" });

                return Ok(new { message = "Customer status toggled successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error toggling customer status",
                    error = ex.Message
                });
            }
        }

        // GET: api/admin/customers/value-segments
        [HttpGet("customers/value-segments")]
        public async Task<IActionResult> GetCustomerValueSegments()
        {
            try
            {
                var segments = await _adminService.GetCustomerValueSegmentsAsync();
                return Ok(segments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching customer value segments",
                    error = ex.Message
                });
            }
        }

        // GET: api/admin/customers/top
        [HttpGet("customers/top")]
        public async Task<IActionResult> GetTopCustomers(
            [FromQuery] int topCount = 10,
            [FromQuery] string orderBy = "Spending")
        {
            try
            {
                var customers = await _adminService.GetTopCustomersAsync(topCount, orderBy);
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching top customers",
                    error = ex.Message
                });
            }
        }

        // GET: api/admin/customers/export
        [HttpGet("customers/export")]
        public async Task<IActionResult> ExportCustomers()
        {
            try
            {
                var customers = await _adminService.ExportCustomerDataAsync();
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error exporting customer data",
                    error = ex.Message
                });
            }
        }

        // Add these endpoints to your existing AdminController class

        // =====================================================
        // ANALYTICS & REPORTS
        // =====================================================

        // GET: api/admin/analytics/sales
        [HttpGet("analytics/sales")]
        public async Task<IActionResult> GetSalesAnalytics([FromQuery] int days = 30)
        {
            try
            {
                var analytics = await _adminService.GetSalesAnalyticsAsync(days);
                return Ok(analytics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching sales analytics",
                    error = ex.Message
                });
            }
        }

        // GET: api/admin/analytics/products
        [HttpGet("analytics/products")]
        public async Task<IActionResult> GetProductAnalytics([FromQuery] int days = 30)
        {
            try
            {
                var analytics = await _adminService.GetProductAnalyticsAsync(days);
                return Ok(analytics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching product analytics",
                    error = ex.Message
                });
            }
        }

        // GET: api/admin/analytics/customers
        [HttpGet("analytics/customers")]
        public async Task<IActionResult> GetCustomerAnalytics([FromQuery] int days = 30)
        {
            try
            {
                var analytics = await _adminService.GetCustomerAnalyticsAsync(days);
                return Ok(analytics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching customer analytics",
                    error = ex.Message
                });
            }
        }

        // GET: api/admin/reports/revenue
        [HttpGet("reports/revenue")]
        public async Task<IActionResult> GetRevenueReports(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                var report = await _adminService.GetRevenueReportsAsync(startDate, endDate);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching revenue reports",
                    error = ex.Message
                });
            }
        }

        // GET: api/admin/reports/inventory
        [HttpGet("reports/inventory")]
        public async Task<IActionResult> GetInventoryReports()
        {
            try
            {
                var report = await _adminService.GetInventoryReportsAsync();
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching inventory reports",
                    error = ex.Message
                });
            }
        }

        // GET: api/admin/reports/fulfillment
        [HttpGet("reports/fulfillment")]
        public async Task<IActionResult> GetOrderFulfillmentReport([FromQuery] int days = 30)
        {
            try
            {
                var report = await _adminService.GetOrderFulfillmentReportAsync(days);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching fulfillment report",
                    error = ex.Message
                });
            }
        }

        // GET: api/admin/reports/comparison
        [HttpGet("reports/comparison")]
        public async Task<IActionResult> GetComparisonReport(
            [FromQuery] DateTime period1Start,
            [FromQuery] DateTime period1End,
            [FromQuery] DateTime period2Start,
            [FromQuery] DateTime period2End)
        {
            try
            {
                var report = await _adminService.GetComparisonReportAsync(
                    period1Start, period1End, period2Start, period2End);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching comparison report",
                    error = ex.Message
                });
            }
        }

        // GET: api/admin/reports/executive-summary
        [HttpGet("reports/executive-summary")]
        public async Task<IActionResult> GetExecutiveSummary()
        {
            try
            {
                var summary = await _adminService.GetExecutiveSummaryAsync();
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching executive summary",
                    error = ex.Message
                });
            }
        }

        // Add these endpoints to your existing AdminController class

        // =====================================================
        // EMAIL NOTIFICATIONS
        // =====================================================

        // GET: api/admin/notifications/history
        [HttpGet("notifications/history")]
        public async Task<IActionResult> GetEmailNotificationHistory(
            [FromQuery] int days = 30,
            [FromQuery] string status = null,
            [FromQuery] string notificationType = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                var result = await _adminService.GetEmailNotificationHistoryAsync(
                    days, status, notificationType, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching email history",
                    error = ex.Message
                });
            }
        }

        // GET: api/admin/notifications/statistics
        [HttpGet("notifications/statistics")]
        public async Task<IActionResult> GetEmailStatistics([FromQuery] int days = 30)
        {
            try
            {
                var stats = await _adminService.GetEmailNotificationStatisticsAsync(days);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching email statistics",
                    error = ex.Message
                });
            }
        }

        // POST: api/admin/notifications/send
        [HttpPost("notifications/send")]
        public async Task<IActionResult> SendEmailNotification([FromBody] CreateEmailNotificationDto dto)
        {
            try
            {
                var notificationId = await _adminService.CreateEmailNotificationAsync(dto);
                return Ok(new { notificationId, message = "Email notification queued" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error creating email notification",
                    error = ex.Message
                });
            }
        }

        // POST: api/admin/notifications/process
        [HttpPost("notifications/process")]
        public async Task<IActionResult> ProcessPendingEmails()
        {
            try
            {
                await _adminService.ProcessPendingEmailsAsync();
                return Ok(new { message = "Pending emails processed" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error processing pending emails",
                    error = ex.Message
                });
            }
        }

        // GET: api/admin/notifications/templates
        [HttpGet("notifications/templates")]
        public async Task<IActionResult> GetEmailTemplates()
        {
            try
            {
                var templates = await _adminService.GetEmailTemplatesAsync();
                return Ok(templates);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error fetching email templates",
                    error = ex.Message
                });
            }
        }

        // POST: api/admin/notifications/templates
        [HttpPost("notifications/templates")]
        public async Task<IActionResult> SaveEmailTemplate([FromBody] SaveEmailTemplateDto dto)
        {
            try
            {
                var success = await _adminService.SaveEmailTemplateAsync(dto);
                if (!success)
                    return BadRequest(new { message = "Failed to save template" });

                return Ok(new { message = "Template saved successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error saving email template",
                    error = ex.Message
                });
            }
        }

        // POST: api/admin/notifications/test
        [HttpPost("notifications/test")]
        public async Task<IActionResult> SendTestEmail([FromBody] SendTestEmailDto dto)
        {
            try
            {
                var success = await _adminService.SendTestEmailAsync(dto);
                if (!success)
                    return BadRequest(new { message = "Failed to send test email" });

                return Ok(new { message = "Test email sent successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error sending test email",
                    error = ex.Message
                });
            }
        }

 


        // POST: api/admin/notifications/order-confirmation/{orderId}
        [HttpPost("notifications/order-confirmation/{orderId}")]
        public async Task<IActionResult> SendOrderConfirmation(int orderId)
        {
            try
            {
                var success = await _adminService.SendOrderConfirmationEmailAsync(orderId);
                if (!success)
                    return BadRequest(new { message = "Failed to send order confirmation" });

                return Ok(new { message = "Order confirmation email sent" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error sending order confirmation",
                    error = ex.Message
                });
            }
        }
    }
}

