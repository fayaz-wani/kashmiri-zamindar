using KashmiriZamindar.Core.Dtos;
using KashmiriZamindar.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace KashmiriZamindar.API.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _service;

        public ProductsController(ProductService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            return Ok(await _service.GetProductsAsync());
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(AddProductDto product)
        {
            var productGuid = await _service.AddProductAsync(product);
            return Ok(new { productGuid });
        }

        [HttpGet("{productGuid}")]
        public async Task<IActionResult> GetProduct(Guid productGuid)
        {
            var product = await _service.GetProductDetailsAsync(productGuid);

            if (product == null)
                return NotFound();

            return Ok(product);
        }

            // ✅ NEW: Get Product Reviews
            [HttpGet("{guid}/reviews")]
            public async Task<IActionResult> GetProductReviews(
                Guid guid,
                [FromQuery] int pageNumber = 1,
                [FromQuery] int pageSize = 10)
            {
                var reviews = await _service.GetProductReviewsAsync(guid, pageNumber, pageSize);
                return Ok(reviews);
            }

            // ✅ NEW: Add Product Review
            [HttpPost("{guid}/reviews")]
            public async Task<IActionResult> AddProductReview(Guid guid, [FromBody] AddReviewDto dto)
            {
                try
                {
                    var reviewId = await _service.AddProductReviewAsync(guid, dto);
                    return Ok(new { reviewId, message = "Review submitted successfully" });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }

            // ✅ NEW: Get Related Products
            [HttpGet("{guid}/related")]
            public async Task<IActionResult> GetRelatedProducts(Guid guid)
            {
                var products = await _service.GetRelatedProductsAsync(guid);
                return Ok(products);
            }
        }
    }

