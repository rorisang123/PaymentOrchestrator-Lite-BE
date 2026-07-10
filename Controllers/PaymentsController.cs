using Microsoft.AspNetCore.Mvc;
using PaymentOrchestrator_Lite_BE.Models;
using PaymentOrchestrator_Lite_BE.Services;

namespace PaymentOrchestrator_Lite_BE.Controllers
{
    [ApiController]
    [Route("api/payments")]
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
            var payment = await _service.CreatePaymentAsync(request);
            return CreatedAtAction(nameof(GetAll), new { id = payment.Id }, payment);
        }

        [HttpGet]
        public async Task<ActionResult<List<Payment>>> GetAll()
        {
            return await _service.GetAllPaymentsAsync();
        }

        [HttpPost("simulate-confirmation/{paymentId:guid}")]
        public async Task<ActionResult<Payment>> SimulateConfirmation(Guid paymentId)
        {
            var payment = await _service.SimulateConfirmationAsync(paymentId);
            return payment != null ? Ok(payment) : NotFound();
        }
    }
}
