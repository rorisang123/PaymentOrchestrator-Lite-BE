using Microsoft.EntityFrameworkCore;
using PaymentOrchestrator_Lite_BE.Data;
using PaymentOrchestrator_Lite_BE.Models;
using PaymentOrchestrator_Lite_BE.Models.Enums;
using PaymentOrchestrator_Lite_BE.Services;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
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
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new AppDbContext(options);
            var service = new PaymentService(context);

            context.Payments.AddRange(
                new Payment { CustomerId = "user123", Amount = 100 },
                new Payment { CustomerId = "user456", Amount = 200 },
                new Payment { CustomerId = "user123", Amount = 300 }
            );
            await context.SaveChangesAsync();

            var payments = await service.GetAllPaymentsAsync("user123");

            Assert.Equal(2, payments.Count);
            Assert.All(payments, p => Assert.Equal("user123", p.CustomerId));
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
