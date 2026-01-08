// API/Controllers/UserOrderController.cs

using KashmiriZamindar.Core.Dtos;
using KashmiriZamindar.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KashmiriZamindar.API.Controllers
{
    [ApiController]
    [Route("api/user/orders")]
    public class UserOrderController : ControllerBase
    {
        private readonly IUserRepository _userRepo;

        public UserOrderController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        // GET: api/user/orders?userGuid=xxx&page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetUserOrders(
            [FromQuery] Guid userGuid,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var orders = await _userRepo.GetUserOrdersAsync(userGuid, page, pageSize);

                return Ok(new UserOrdersResponse
                {
                    Orders = orders,
                    TotalOrders = orders.FirstOrDefault()?.ItemCount ?? 0,
                    Page = page,
                    PageSize = pageSize
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching orders", error = ex.Message });
            }
        }

        // GET: api/user/orders/{orderGuid}?userGuid=xxx
        [HttpGet("{orderGuid}")]
        public async Task<IActionResult> GetOrderDetails(
            Guid orderGuid,
            [FromQuery] Guid userGuid)
        {
            try
            {
                var order = await _userRepo.GetUserOrderDetailsAsync(orderGuid, userGuid);

                if (order == null)
                    return NotFound(new { message = "Order not found" });

                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching order details", error = ex.Message });
            }
        }
    }
}