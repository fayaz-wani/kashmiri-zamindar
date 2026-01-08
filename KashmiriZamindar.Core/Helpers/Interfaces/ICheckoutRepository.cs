using KashmiriZamindar.Core.Entities;
using System.Threading.Tasks;

namespace KashmiriZamindar.Core.Interfaces
{
    public interface ICheckoutRepository
    {
        // ✅ Updated to match new CheckoutService
        Task<int> CreateOrGetCustomerAsync(Customer customer);
        Task<int> CreateOrderAsync(Order order);
        Task CreateOrderItemAsync(OrderItem orderItem);
        Task<Order> GetOrderByIdAsync(int orderId);
        Task UpdateOrderPaymentStatusAsync(int orderId, string paymentStatus);
    }
}