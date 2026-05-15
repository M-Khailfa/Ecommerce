using Ecommerce.Core.Dtos.Payment;
using Ecommerce.Core.Interfaces;
using Ecommerce.Core.Settings;
using Ecommerce.Infrastructure.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetByOrderAsync(int orderId)
        {
            var response = new ApiResponse();
            var payment = await _paymentService.GetByOrderAsync(orderId);
            if (payment is null)
            {
                response = ApiResponse.NotFound($"No payment found for order ID {orderId}.");
                return NotFound(response);
            }
            response = ApiResponse.Success(payment, $"Payment details for order ID {orderId} retrieved successfully.");
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Initiate(CreatePaymentDto dto)
        {
            var response = new ApiResponse();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var (result, error) = await _paymentService.InitiateAsync(userId, dto);

            if (error == "Order not found.")
            {
                response = ApiResponse.NotFound(error);
                return NotFound(response);
            }
            if (error == "Forbidden.")
            {
                return Forbid();
            }
            if (error is not null)
            {
                response = ApiResponse.BadRequest(error);
                return BadRequest(response);
            }

            response = ApiResponse.Success(result, "Payment intent created successfully.");
            return Ok(response);
        }
    }
}
