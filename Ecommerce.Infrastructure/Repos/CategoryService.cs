using Ecommerce.Core.Dtos.Category;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Infrastructure.Repos
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name
            })
            .ToListAsync();

            return categories;

        }
        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            return category is null ? null : new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            var category = new Category { Name = createCategoryDto.Name };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return new CategoryDto { Id = category.Id, Name = category.Name };
        }

        public async Task<CategoryDto?> UpdateCategoryAsync(int id, CreateCategoryDto createCategoryDto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category is null)
                return null;
            category.Name = createCategoryDto.Name;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return new CategoryDto { Id = category.Id, Name = category.Name };
        }

        public async Task<bool?> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category is null) return null;

            var hasProducts = await _context.Products.AnyAsync(p => p.CategoryId == id);
            if (hasProducts) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
