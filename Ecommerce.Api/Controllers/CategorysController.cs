using Ecommerce.Core.Dtos.Category;
using Ecommerce.Core.Interfaces;
using Ecommerce.Core.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategorysController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategorysController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = new ApiResponse();
            var result = await _categoryService.GetAllCategoriesAsync();
            if (result is null || !result.Any())
            {
                response = ApiResponse.NotFound("No Categories Found!");
                return NotFound(response);
            }
            response = ApiResponse.Success(result, "Categories Retrieved Successfully!");
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = new ApiResponse();
            var result = await _categoryService.GetCategoryByIdAsync(id);
            if (result is null)
            {
                response = ApiResponse.NotFound("Category Not Found!");
                return NotFound(response);
            }
            response = ApiResponse.Success(result, "Category Retrieved Successfully!");
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateCategoryDto createCategoryDto)
        {
            var response = new ApiResponse();
            var result = await _categoryService.CreateCategoryAsync(createCategoryDto);
            response = ApiResponse.Created(result, "Category Created Successfully!");
            return Ok(response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, CreateCategoryDto updateCategoryDto)
        {
            var response = new ApiResponse();
            var result = await _categoryService.UpdateCategoryAsync(id, updateCategoryDto);
            if (result is null)
            {
                response = ApiResponse.NotFound("Category Not Found!");
                return NotFound(response);
            }
            response = ApiResponse.Success(result, "Category Updated Successfully!");
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = new ApiResponse();
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (result is null)
            {
                response = ApiResponse.NotFound("Category Not Found!");
                return NotFound(response);
            }
            if(!result.Value)
            {
                response = ApiResponse.BadRequest("Cannot delete a category that has products.");
                return Conflict(response);
            }
            response = ApiResponse.Success(result, "Category Deleted Successfully!");
            return Ok(response);
        }
    }
}
