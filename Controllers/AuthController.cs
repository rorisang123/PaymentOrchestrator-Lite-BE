using Microsoft.AspNetCore.Mvc;
using PaymentOrchestrator_Lite_BE.Services;
using static PaymentOrchestrator_Lite_BE.Models.Auth;

namespace PaymentOrchestrator_Lite_BE.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            var response = await _authService.Register(request);
            return response != null ? Ok(response) : BadRequest(new { message = "User with this email already exists" });
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            var response = await _authService.Login(request);
            return response != null ? Ok(response) : Unauthorized(new { message = "Invalid email or password" });
        }
    }
}
