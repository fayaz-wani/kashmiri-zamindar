// ============================================
// Inventory & Orders DTOs
// ============================================
using KashmiriZamindar.Core.Dtos;
using System.Data;

namespace KashmiriZamindar.Core.Dtos
{
    // Inventory DTOs
    public class InventoryItemDto
    {
        public Guid ProductGuid { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int LowStockThreshold { get; set; }
        public string ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public string StockStatus { get; set; }
        public int TotalSold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class UpdateStockDto
    {
        public int QuantityChange { get; set; }
        public string ChangeType { get; set; } // 'Purchase', 'Sale', 'Adjustment', 'Return'
        public string Reason { get; set; }
    }

    public class InventoryHistoryDto
    {
        public int HistoryId { get; set; }
        public Guid ProductGuid { get; set; }
        public string ProductName { get; set; }
        public string ChangeType { get; set; }
        public int QuantityChange { get; set; }
        public int PreviousQuantity { get; set; }
        public int NewQuantity { get; set; }
        public string Reason { get; set; }
        public string ChangedBy { get; set; }
        public DateTime ChangedAt { get; set; }
    }

    public class LowStockAlertDto
    {
        public Guid ProductGuid { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public int StockQuantity { get; set; }
        public int LowStockThreshold { get; set; }
        public string ImageUrl { get; set; }
        public string StockStatus { get; set; }
    }



}
