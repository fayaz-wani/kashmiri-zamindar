using KashmiriZamindar.Core.Dtos;
using KashmiriZamindar.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace KashmiriZamindar.API.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        // GET: api/cart?sessionId=xxx&userGuid=xxx
        [HttpGet]
        public async Task<IActionResult> GetCart([FromQuery] string sessionId, [FromQuery] Guid? userGuid)
        {
            try
            {
                // Priority: UserGuid > SessionId
                var cart = await _cartService.GetCartAsync(sessionId, userGuid);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/cart
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            try
            {
                var cartItem = await _cartService.AddToCartAsync(dto);
                return Ok(cartItem);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/cart/items/{cartItemId}?sessionId=xxx&userGuid=xxx
        [HttpPut("items/{cartItemId}")]
        public async Task<IActionResult> UpdateQuantity(
            int cartItemId,
            [FromQuery] string sessionId,
            [FromQuery] Guid? userGuid,
            [FromBody] UpdateCartItemDto dto)
        {
            try
            {
                var success = await _cartService.UpdateQuantityAsync(sessionId, userGuid, cartItemId, dto.Quantity);
                if (!success)
                    return NotFound(new { message = "Cart item not found" });

                return Ok(new { message = "Quantity updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/cart/items/{cartItemId}?sessionId=xxx&userGuid=xxx
        [HttpDelete("items/{cartItemId}")]
        public async Task<IActionResult> RemoveItem(
            int cartItemId,
            [FromQuery] string sessionId,
            [FromQuery] Guid? userGuid)
        {
            try
            {
                var success = await _cartService.RemoveItemAsync(sessionId, userGuid, cartItemId);
                if (!success)
                    return NotFound(new { message = "Cart item not found" });

                return Ok(new { message = "Item removed from cart" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/cart?sessionId=xxx&userGuid=xxx
        [HttpDelete]
        public async Task<IActionResult> ClearCart(
            [FromQuery] string sessionId,
            [FromQuery] Guid? userGuid)
        {
            try
            {
                var success = await _cartService.ClearCartAsync(sessionId, userGuid);
                return Ok(new { message = "Cart cleared successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/cart/merge
        [HttpPost("merge")]
        public async Task<IActionResult> MergeCart([FromBody] MergeCartDto dto)
        {
            try
            {
                var success = await _cartService.MergeGuestCartToUserAsync(dto.GuestSessionId, dto.UserGuid);
                return Ok(new { message = "Cart merged successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}