// ============================================
// Product Management DTOs
// ============================================
using KashmiriZamindar.Core.Dtos;
using System.Data;

namespace KashmiriZamindar.Core.Dtos
{
    public class AdminProductDto
    {
        public Guid ProductGuid { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public string Unit { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public int TotalSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<ProductImageDto> Images { get; set; } = new();
    }






}

