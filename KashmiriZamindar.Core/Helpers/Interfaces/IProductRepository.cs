using KashmiriZamindar.Core.Entities;
using KashmiriZamindar.Core.Dtos;

namespace KashmiriZamindar.Core.Interfaces
{
    public interface IProductRepository
    {
        Task<List<ProductDetailDto>> GetAllProductsAsync();
        Task<Product?> GetByGuidAsync(Guid productGuid);

        Task<Guid> AddProductAsync(AddProductDto product);
        Task<ProductDetailDto?> GetProductWithImagesAsync(Guid productGuid);
        // Reviews
        Task<List<ProductReviewDto>> GetProductReviewsAsync(Guid productGuid, int pageNumber, int pageSize);
        Task<int> AddProductReviewAsync(Guid productGuid, AddReviewDto dto);

        // Related products (optional separate call)
        Task<List<RelatedProductDto>> GetRelatedProductsAsync(Guid productGuid);

    }
}
