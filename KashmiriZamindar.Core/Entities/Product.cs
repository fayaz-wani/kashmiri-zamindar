// Core/Entities/Product.cs
using System;

namespace KashmiriZamindar.Core.Entities
{
    public class Product
    {
        public int ProductId { get; set; }
        public Guid ProductGuid { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public string Unit { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}