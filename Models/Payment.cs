using PaymentOrchestrator_Lite_BE.Models.Enums;
using System.Text.Json.Serialization;

namespace PaymentOrchestrator_Lite_BE.Models
{
    public class Payment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string CustomerId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public record CreatePaymentRequest(string CustomerId, decimal Amount);
}
