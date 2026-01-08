// ============================================
// API/Controllers/PaymentController.cs
// ============================================
using KashmiriZamindar.Core.Dtos.Payment;
using KashmiriZamindar.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace KashmiriZamindar.API.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // ✅ Create Razorpay Order
        [HttpPost("create-order")]
        public async Task<IActionResult> CreatePaymentOrder([FromBody] CreatePaymentOrderDto dto)
        {
            try
            {
                var result = await _paymentService.CreateRazorpayOrderAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ✅ Verify Payment
        [HttpPost("verify")]
        public async Task<IActionResult> VerifyPayment([FromBody] VerifyPaymentDto dto)
        {
            try
            {
                var result = await _paymentService.VerifyPaymentAsync(dto);

                if (result.IsValid)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ✅ Get Payment Status
        [HttpGet("status/{orderId}")]
        public async Task<IActionResult> GetPaymentStatus(int orderId)
        {
            try
            {
                var result = await _paymentService.GetPaymentStatusAsync(orderId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}