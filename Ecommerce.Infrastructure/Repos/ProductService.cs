using Ecommerce.Core.Dtos.Product;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Infrastructure.Repos
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly IImageService _imageService;
        public ProductService(AppDbContext context, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync(string? search, int? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Products
            .Include(p => p.Category)
            .Where(p => p.IsActive)
            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p =>
                    p.Name.Contains(search) ||
                    (p.Brand != null && p.Brand.Contains(search)) ||
                    (p.Description != null && p.Description.Contains(search)));

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            return await query.Select(p => ToDto(p)).ToListAsync();
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
            return product is null ? null : ToDto(product);
        }

        public async Task<IEnumerable<ProductDto>> GetByCategoryIdAsync(int categoryId)
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId && p.IsActive)
                .Select(p => ToDto(p))
                .ToListAsync();
            return products;
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                CategoryId = dto.CategoryId,
                Brand = dto.Brand,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                ImageUrl = await _imageService.UploadImageAsync(dto.Image),
                IsActive = true
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            await _context.Entry(product).Reference(p => p.Category).LoadAsync();

            return ToDto(product);
        }

        public async Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto)
        {
            var product = await _context.Products.FindAsync(id);

            if (product is null) 
               return null;

            product.Name = dto.Name;
            product.CategoryId = dto.CategoryId;
            if(!string.IsNullOrEmpty(dto.Brand))
                product.Brand = dto.Brand;
            if (!string.IsNullOrEmpty(dto.Description))
                product.Description = dto.Description;
            if(dto.Price >= 0)
                product.Price = dto.Price;
            if(dto.Stock >= 0)
                product.Stock = dto.Stock;
            if(dto.Image is not null)
                product.ImageUrl = await _imageService.UploadImageAsync(dto.Image);
            product.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();
            await _context.Entry(product).Reference(p => p.Category).LoadAsync();

            return ToDto(product);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product is null || !product.IsActive)
                return false;

            product.IsActive = false;

            await _context.SaveChangesAsync();
            return true;
        }

        private static ProductDto ToDto(Product p) => new(
            p.Id, p.Name, p.Brand, p.Description,
            p.Price, p.Stock, p.ImageUrl, p.IsActive,
            p.CategoryId, p.Category.Name
        );
    }
}
