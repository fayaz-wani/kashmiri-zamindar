// API/Controllers/CheckoutController.cs
using KashmiriZamindar.Core.Dtos;
using KashmiriZamindar.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace KashmiriZamindar.API.Controllers
{
    [ApiController]
    [Route("api/checkout")]
    public class CheckoutController : ControllerBase
    {
        private readonly CheckoutService _service;

        public CheckoutController(CheckoutService service)
        {
            _service = service;
        }

        [HttpPost("process")]
        public async Task<IActionResult> ProcessCheckout([FromBody] CheckoutRequestDto checkout)
        {
            try
            {
                var paymentType = checkout.PaymentInfo.PaymentType?.ToUpper();

                // ✅ Razorpay rule: NEVER validate card details
                if (paymentType == "COD")
                {
                    checkout.PaymentInfo.CardNumber = null;
                    checkout.PaymentInfo.CardExpiry = null;
                    checkout.PaymentInfo.CardCVV = null;
                    checkout.PaymentInfo.NameOnCard = null;
                }

                // CARD / UPI / ONLINE → handled by Razorpay popup
                var result = await _service.ProcessCheckoutAsync(checkout);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}