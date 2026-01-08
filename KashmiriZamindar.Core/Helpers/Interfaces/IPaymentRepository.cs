using KashmiriZamindar.Core.Entities;

public interface IPaymentRepository
{
    Task<PaymentTransaction> CreateTransactionAsync(PaymentTransaction transaction);
    Task<PaymentTransaction> GetByRazorpayOrderIdAsync(string razorpayOrderId);
    Task<PaymentTransaction> GetByOrderIdAsync(int orderId);
    Task UpdateTransactionAsync(PaymentTransaction transaction);
    Task<bool> VerifyPaymentSignatureAsync(string orderId, string paymentId, string signature, string secret);
}