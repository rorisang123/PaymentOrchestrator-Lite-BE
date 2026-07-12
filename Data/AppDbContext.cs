using Microsoft.EntityFrameworkCore;
using PaymentOrchestrator_Lite_BE.Models;

namespace PaymentOrchestrator_Lite_BE.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Payment> Payments => Set<Payment>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("PaymentsDb");
        }
    }
}
