using Microsoft.EntityFrameworkCore;
using PaymentOrchestrator_Lite_BE.Data;
using PaymentOrchestrator_Lite_BE.Models;
using PaymentOrchestrator_Lite_BE.Services;
using Xunit;
using static PaymentOrchestrator_Lite_BE.Models.Auth;

namespace PaymentOrchestrator_Lite_BE.PayOrch.Tests.Unit
{
    public class AuthServiceTests
    {
        private readonly AppDbContext _context;
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            var configValues = new Dictionary<string, string?>
            {
                { "Jwt:Key", "SuperSecretTestKeyForUnitTests12345" },
                { "Jwt:Issuer", "TestIssuer" },
                { "Jwt:Audience", "TestAudience" }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues)
                .Build();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _service = new AuthService(_context, configuration);
        }

        [Fact]
        public async Task Register_ShouldCreateUserAndReturnToken()
        {
            var request = new RegisterRequest(
                "testuser",
                "test@example.com",
                "Password123!"
            );

            var result = await _service.Register(request);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Token);
            Assert.Equal("testuser", result.Username);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == "test@example.com");

            Assert.NotNull(user);
            Assert.Equal("testuser", user.Username);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Password123!");

            var user = new User
            {
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = hashedPassword
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var request = new LoginRequest(
                "test@example.com",
                "Password123!"
            );

            var result = await _service.Login(request);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Token);
            Assert.Equal(user.Username, result.Username);
            Assert.Equal(user.Id.ToString(), result.UserId);
        }

        [Fact]
        public async Task Login_InvalidPassword_ReturnsNull()
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Password123!");

            _context.Users.Add(new User
            {
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = hashedPassword
            });

            await _context.SaveChangesAsync();

            var request = new LoginRequest(
                "test@example.com",
                "WrongPassword"
            );

            var result = await _service.Login(request);

            Assert.Null(result);
        }

        [Fact]
        public async Task Register_DuplicateEmail_ReturnsNull()
        {
            _context.Users.Add(new User
            {
                Username = "existinguser",
                Email = "test@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!")
            });

            await _context.SaveChangesAsync();

            var request = new RegisterRequest(
                "newuser",
                "test@example.com",
                "Password123!"
            );

            var result = await _service.Register(request);

            Assert.Null(result);
        }
    }
}