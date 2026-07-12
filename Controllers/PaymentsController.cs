using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentOrchestrator_Lite_BE.Models;
using PaymentOrchestrator_Lite_BE.Services;
using System.Security.Claims;

namespace PaymentOrchestrator_Lite_BE.Controllers
{
    [ApiController]
    [Route("api/payments")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _service;

        public PaymentsController(IPaymentService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<Payment>> Create([FromBody] CreatePaymentRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var payment = await _service.CreatePaymentAsync(request, userId);
            return CreatedAtAction(nameof(GetAll), new { id = payment.Id }, payment);
        }

        [HttpGet]
        public async Task<ActionResult<List<Payment>>> GetAll()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            return await _service.GetAllPaymentsAsync(userId);
        }

        [HttpPost("simulate-confirmation/{paymentId:guid}")]
        public async Task<ActionResult<Payment>> SimulateConfirmation(Guid paymentId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var payment = await _service.SimulateConfirmationAsync(paymentId, userId);
            return payment != null ? Ok(payment) : NotFound();
        }
    }
}
