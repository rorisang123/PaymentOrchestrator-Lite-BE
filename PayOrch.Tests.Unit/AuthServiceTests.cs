using Microsoft.EntityFrameworkCore;
using Moq;
using PaymentOrchestrator_Lite_BE.Data;
using PaymentOrchestrator_Lite_BE.Models;
using PaymentOrchestrator_Lite_BE.Services;
using Xunit;
using static PaymentOrchestrator_Lite_BE.Models.Auth;

namespace PaymentOrchestrator_Lite_BE.PayOrch.Tests.Unit
{
    public class AuthServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly AppDbContext _context;
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            _mockConfig = new Mock<IConfiguration>();
            _mockConfig.Setup(c => c["Jwt:Key"]).Returns("SuperSecretTestKeyForUnitTests12345");
            _mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
            _mockConfig.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _service = new AuthService(_context, _mockConfig.Object);
        }

        [Fact]
        public async Task Register_ShouldCreateUserAndReturnToken()
        {
            var request = new RegisterRequest("testuser", "test@example.com", "Password123!");

            var result = await _service.Register(request);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Token);
            Assert.Equal("testuser", result.Username);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            // Seed user
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Password123!");
            var user = new User
            {
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = hashedPassword
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var request = new LoginRequest("test@example.com", "Password123!");

            var result = await _service.Login(request);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Token);
        }
    }
}
