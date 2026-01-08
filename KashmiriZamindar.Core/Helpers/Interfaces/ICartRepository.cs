using KashmiriZamindar.Core.Dtos;

namespace KashmiriZamindar.Core.Interfaces
{
    public interface ICartRepository
    {
        // Get cart by SessionId (Guest) or UserGuid (Logged-in)
        Task<CartDto> GetCartAsync(string sessionId, Guid? userGuid);

        // Add item to cart
        Task<CartItemDto> AddToCartAsync(AddToCartDto dto);

        // Update quantity
        Task<bool> UpdateCartItemQuantityAsync(string sessionId, Guid? userGuid, int cartItemId, int quantity);

        // Remove item
        Task<bool> RemoveCartItemAsync(string sessionId, Guid? userGuid, int cartItemId);

        // Clear cart
        Task<bool> ClearCartAsync(string sessionId, Guid? userGuid);

        // Merge guest cart into user cart when user logs in
        Task<bool> MergeGuestCartToUserAsync(string guestSessionId, Guid userGuid);
    }
}