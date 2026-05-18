using Ecommerce.Core.Dtos.Order;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces;
using Ecommerce.Core.Settings;
using Ecommerce.Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyOrders()
        {
            var response = new ApiResponse();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _orderService.GetMyOrdersAsync(userId);
            response = ApiResponse.Success(result, "Orders retrieved successfully.");
            return Ok(response);
        }

        
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var response = new ApiResponse();
            var result = await _orderService.GetByIdAsync(id);
            if (result is null)
                response = ApiResponse.NotFound("Order not found.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (result.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            response = ApiResponse.Success(result, "Order retrieved successfully.");
            return Ok(response);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PlaceOrder(PlaceOrderDto dto)
        {
            var response = new ApiResponse();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var (order, errors) = await _orderService.PlaceOrderAsync(userId, dto);

            if (errors.Count > 0)
            {
                response = ApiResponse.BadRequest(errors);
                return BadRequest(response);
            }

            response = ApiResponse.Created(order, "Order placed successfully.");
            return Ok(response);
        }

        [HttpPatch("{id:int}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateOrderStatusDto dto)
        {
            var response = new ApiResponse();
            var updated = await _orderService.UpdateStatusAsync(id, dto.Status);

            if(!updated)
            {
                response = ApiResponse.NotFound("Order not found.");
                return NotFound(response);
            }

            response = ApiResponse.Updated("Order status updated successfully.");
            return Ok(response);
        }
    }
}
