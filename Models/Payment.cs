using PaymentOrchestrator_Lite_BE.Models.Enums;

namespace PaymentOrchestrator_Lite_BE.Models
{
    public class Payment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string CustomerId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending; // Pending, Confirmed
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public record CreatePaymentRequest(string CustomerId, decimal Amount);
}
