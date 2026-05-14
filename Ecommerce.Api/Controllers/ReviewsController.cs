using Ecommerce.Core.Dtos.Reviews;
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
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProductAsync(int productId)
        {
            var response = new ApiResponse();
            var reviews = await _reviewService.GetByProductAsync(productId);

            response = ApiResponse.Success(reviews, "Reviews retrieved successfully.");
            return Ok(response);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateReviewDto dto)
        {
            var response = new ApiResponse();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var (review, error) = await _reviewService.CreateAsync(userId, dto);

            if (error == "Product not found.")
            {
                response = ApiResponse.NotFound(error);
                return NotFound(response);

            }

            if (error is not null)
            {
                response = ApiResponse.Conflict(error);
                return Conflict(response);
            }

            response = ApiResponse.Created(review, "Review created successfully.");
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var response = new ApiResponse();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var isAdmin = User.IsInRole("Admin");
            var deleted = await _reviewService.DeleteAsync(id, userId, isAdmin);

            if(!deleted)
            {
                response = ApiResponse.BadRequest("Review not found or you don't have permission to delete it.");
                return BadRequest(response);
            }

            response = ApiResponse.Success("Review deleted successfully.");
            return Ok(response);
        }
    }
}
