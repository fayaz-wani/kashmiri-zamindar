using KashmiriZamindar.Core.Dtos;
using KashmiriZamindar.Core.Interfaces;

namespace KashmiriZamindar.Core.Services
{
    public class CartService
    {
        private readonly ICartRepository _cartRepository;

        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<CartDto> GetCartAsync(string sessionId, Guid? userGuid)
        {
            return await _cartRepository.GetCartAsync(sessionId, userGuid);
        }

        public async Task<CartItemDto> AddToCartAsync(AddToCartDto dto)
        {
            if (dto.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0");

            // Ensure either sessionId or userGuid is provided
            if (string.IsNullOrEmpty(dto.SessionId) && !dto.UserGuid.HasValue)
                throw new ArgumentException("Either SessionId or UserGuid must be provided");

            return await _cartRepository.AddToCartAsync(dto);
        }

        public async Task<bool> UpdateQuantityAsync(string sessionId, Guid? userGuid, int cartItemId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0");

            return await _cartRepository.UpdateCartItemQuantityAsync(sessionId, userGuid, cartItemId, quantity);
        }

        public async Task<bool> RemoveItemAsync(string sessionId, Guid? userGuid, int cartItemId)
        {
            return await _cartRepository.RemoveCartItemAsync(sessionId, userGuid, cartItemId);
        }

        public async Task<bool> ClearCartAsync(string sessionId, Guid? userGuid)
        {
            return await _cartRepository.ClearCartAsync(sessionId, userGuid);
        }

        public async Task<bool> MergeGuestCartToUserAsync(string guestSessionId, Guid userGuid)
        {
            if (string.IsNullOrEmpty(guestSessionId))
                throw new ArgumentException("Guest session ID is required");

            return await _cartRepository.MergeGuestCartToUserAsync(guestSessionId, userGuid);
        }
    }
}