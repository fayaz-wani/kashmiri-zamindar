using KashmiriZamindar.Core.Dtos;
using KashmiriZamindar.Core.Entities;
using KashmiriZamindar.Core.Interfaces;

namespace KashmiriZamindar.Core.Services
{
    public class ProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

  
        public async Task<List<ProductDetailDto>> GetProductsAsync()
        {
            return await _repository.GetAllProductsAsync();
        }
        public async Task<Guid> AddProductAsync(AddProductDto product)
        {
            return await _repository.AddProductAsync(product);
        }
        public async Task<ProductDetailDto?> GetProductDetailsAsync(Guid productGuid)
        {
            return await _repository.GetProductWithImagesAsync(productGuid);
        }
        // Reviews
        public async Task<List<ProductReviewDto>> GetProductReviewsAsync(
            Guid productGuid,
            int pageNumber,
            int pageSize)
        {
            return await _repository.GetProductReviewsAsync(productGuid, pageNumber, pageSize);
        }

        public async Task<int> AddProductReviewAsync(Guid productGuid, AddReviewDto dto)
        {
            return await _repository.AddProductReviewAsync(productGuid, dto);
        }

        // Related products
        public async Task<List<RelatedProductDto>> GetRelatedProductsAsync(Guid productGuid)
        {
            return await _repository.GetRelatedProductsAsync(productGuid);
        }


    }
}

