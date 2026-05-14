using Ecommerce.Core.Dtos.Product;
using Ecommerce.Core.Interfaces;
using Ecommerce.Core.Settings;
using Ecommerce.Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductssController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductssController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null)
        {
            var response = new ApiResponse();
            var products = await _productService.GetAllAsync(search, categoryId, minPrice, maxPrice);
            response = ApiResponse.Success(products, "Products retrieved successfully.");
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = new ApiResponse();
            var product = await _productService.GetByIdAsync(id);
            if (product is null)
            {
                response = ApiResponse.NotFound($"Product with ID {id} not found.");
                return NotFound(response);
            }
            response = ApiResponse.Success(product, "Product retrieved successfully.");
            return Ok(response);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategoryId(int categoryId)
        {
            var response = new ApiResponse();
            var products = await _productService.GetByCategoryIdAsync(categoryId);
            if (products is null || !products.Any())
            {
                response = ApiResponse.NotFound($"No products found for category ID {categoryId}.");
                return NotFound(response);
            }
            response = ApiResponse.Success(products, "Products retrieved successfully.");
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromForm] CreateProductDto dto)
        {
            var response = new ApiResponse();
            var result = await _productService.CreateAsync(dto);
            if (result is null)
            {
                response = ApiResponse.BadRequest("Failed to create product.");
                return BadRequest(response);
            }
            response = ApiResponse.Success(result, "Product created successfully.");
            return Ok(response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateProductDto dto)
        {
            var response = new ApiResponse();
            var result = await _productService.UpdateAsync(id, dto);
            if (result is null)
            {
                response = ApiResponse.NotFound($"Product with ID {id} not found.");
                return NotFound(response);
            }
            response = ApiResponse.Success(result, "Product updated successfully.");
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = new ApiResponse();
            var result = await _productService.DeleteAsync(id);
            if (result is null)
            {
                response = ApiResponse.NotFound($"Product with ID {id} not found.");
                return NotFound(response);
            }
            if (result == false)
            {
                response = ApiResponse.BadRequest("Failed to delete product.");
                return BadRequest(response);
            }
            response = ApiResponse.Success("Product deleted successfully.");
            return Ok(response);
        }
    }
}
