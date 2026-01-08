using KashmiriZamindar.Core.Dtos;
using System.Data;

namespace KashmiriZamindar.Core.Dtos
{
    public class CreateProductDto
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public string Unit { get; set; }
        public string Description { get; set; }
        public List<ProductImageUploadDto> Images { get; set; } = new();
    }
}