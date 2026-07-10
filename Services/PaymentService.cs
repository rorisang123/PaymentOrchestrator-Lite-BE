using PaymentOrchestrator_Lite_BE.Data;
using PaymentOrchestrator_Lite_BE.Models;
using PaymentOrchestrator_Lite_BE.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace PaymentOrchestrator_Lite_BE.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;

        public PaymentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Payment> CreatePaymentAsync(CreatePaymentRequest request)
        {
            var payment = new Payment
            {
                CustomerId = request.CustomerId,
                Amount = request.Amount
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<List<Payment>> GetAllPaymentsAsync()
        {
            return await _context.Payments
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Payment?> SimulateConfirmationAsync(Guid paymentId)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment != null && payment.Status == PaymentStatus.Pending)
            {
                payment.Status = PaymentStatus.Confirmed;
                await _context.SaveChangesAsync();
            }
            return payment;
        }
    }
}
