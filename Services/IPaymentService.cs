using PaymentOrchestrator_Lite_BE.Models;

namespace PaymentOrchestrator_Lite_BE.Services
{
    public interface IPaymentService
    {
        Task<Payment> CreatePaymentAsync(CreatePaymentRequest request, string userId);
        Task<List<Payment>> GetAllPaymentsAsync(string userId);
        Task<Payment?> SimulateConfirmationAsync(Guid paymentId, string userId);
    }
}
