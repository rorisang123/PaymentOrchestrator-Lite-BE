namespace PaymentOrchestrator_Lite_BE.Models
{
    public class Auth
    {
        public record RegisterRequest(string Username, string Email, string Password);
        public record LoginRequest(string Email, string Password);
        public record AuthResponse(string Token, string UserId, string Username);
    }
}
