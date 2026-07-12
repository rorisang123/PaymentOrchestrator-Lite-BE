using Microsoft.EntityFrameworkCore;
using PaymentOrchestrator_Lite_BE.Data;
using PaymentOrchestrator_Lite_BE.Models;
using PaymentOrchestrator_Lite_BE.Models.Enums;
using PaymentOrchestrator_Lite_BE.Services;
using Xunit;

namespace PaymentOrchestrator_Lite_BE.PayOrch.Tests.Unit
{
    public class PaymentServiceTests
    {
        private AppDbContext GetDbContext() => new(new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .Options);

        [Fact]
        public async Task CreatePaymentAsync_ShouldCreatePaymentForUser()
        {
            using var context = GetDbContext();
            var service = new PaymentService(context);
            var request = new CreatePaymentRequest("testuser", 250.75m);

            var payment = await service.CreatePaymentAsync(request, "user123");

            Assert.NotNull(payment);
            Assert.Equal("user123", payment.CustomerId);
            Assert.Equal(250.75m, payment.Amount);
            Assert.Equal(PaymentStatus.Pending, payment.Status);
        }

        [Fact]
        public async Task GetAllPaymentsAsync_ShouldReturnOnlyUserPayments()
        {
            using var context = GetDbContext();
            context.Payments.AddRange(
                new Payment { CustomerId = "user123", Amount = 100 },
                new Payment { CustomerId = "user456", Amount = 200 }
            );
            await context.SaveChangesAsync();

            var service = new PaymentService(context);
            var payments = await service.GetAllPaymentsAsync("user123");

            Assert.Single(payments);
            Assert.Equal("user123", payments[0].CustomerId);
        }

        [Fact]
        public async Task SimulateConfirmationAsync_ShouldConfirmOwnPayment()
        {
            using var context = GetDbContext();
            var paymentId = Guid.NewGuid();
            context.Payments.Add(new Payment
            {
                Id = paymentId,
                CustomerId = "user123",
                Status = PaymentStatus.Pending
            });
            await context.SaveChangesAsync();

            var service = new PaymentService(context);
            var result = await service.SimulateConfirmationAsync(paymentId, "user123");

            Assert.NotNull(result);
            Assert.Equal(PaymentStatus.Confirmed, result.Status);
        }
    }
}
