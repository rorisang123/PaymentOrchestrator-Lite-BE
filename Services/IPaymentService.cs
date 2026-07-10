using PaymentOrchestrator_Lite_BE.Models;

namespace PaymentOrchestrator_Lite_BE.Services
{
    public interface IPaymentService
    {
        Task<Payment> CreatePaymentAsync(CreatePaymentRequest request);
        Task<List<Payment>> GetAllPaymentsAsync();
        Task<Payment?> SimulateConfirmationAsync(Guid paymentId);
    }
}
