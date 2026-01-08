// Core/Dtos/ProductDetailDto.cs
namespace KashmiriZamindar.Core.Dtos
{
    public class ProductDetailDto
    {
        public Guid ProductGuid { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public string Unit { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int StockQuantity { get; set; }
        public int LowStockThreshold { get; set; }
        public bool IsActive { get; set; }

        // Images
        public List<ProductImageDto> Images { get; set; }

        // Reviews
        public int TotalReviews { get; set; }
        public double AverageRating { get; set; }
        public RatingDistributionDto RatingDistribution { get; set; }

        // Related Products
        public List<RelatedProductDto> RelatedProducts { get; set; }
    }

    public class ProductImageDto
    {
        public string ImageUrl { get; set; }
        public bool IsPrimary { get; set; }
        public int ImageId { get; set; }

    }

    public class RatingDistributionDto
    {
        public int Rating5Count { get; set; }
        public int Rating4Count { get; set; }
        public int Rating3Count { get; set; }
        public int Rating2Count { get; set; }
        public int Rating1Count { get; set; }
    }

    public class RelatedProductDto
    {
        public Guid ProductGuid { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public string Unit { get; set; }
        public string ImageUrl { get; set; }
    }

    public class ProductReviewDto
    {
        public int ReviewId { get; set; }
        public string CustomerName { get; set; }
        public int Rating { get; set; }
        public string ReviewTitle { get; set; }
        public string ReviewText { get; set; }
        public bool IsVerifiedPurchase { get; set; }
        public int HelpfulCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AddReviewDto
    {
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public int Rating { get; set; }
        public string ReviewTitle { get; set; }
        public string ReviewText { get; set; }
    }
}