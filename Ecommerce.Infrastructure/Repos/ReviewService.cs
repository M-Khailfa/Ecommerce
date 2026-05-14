using Ecommerce.Core.Dtos.Reviews;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Infrastructure.Repos
{
    public class ReviewService : IReviewService
    {
        private readonly AppDbContext _context;

        public ReviewService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReviewDto>> GetByProductAsync(int productId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => ToDto(r))
                .ToListAsync();
            return reviews;
        }

        public async Task<(ReviewDto? Review, string? Error)> CreateAsync(string userId, CreateReviewDto dto)
        {
            var productExists = await _context.Products
            .AnyAsync(p => p.Id == dto.ProductId && p.IsActive);

            if (!productExists)
                return (null, "Product not found.");

            var alreadyReviewed = await _context.Reviews
                .AnyAsync(r => r.ProductId == dto.ProductId && r.UserId == userId);

            if (alreadyReviewed)
                return (null, "You have already reviewed this product.");

            var review = new Review
            {
                ProductId = dto.ProductId,
                UserId = userId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            await _context.Entry(review).Reference(r => r.User).LoadAsync();

            return (ToDto(review), null);
        }

        public async Task<bool> DeleteAsync(int id, string userId, bool isAdmin)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review is null)
                return false;

            if (review.UserId != userId && !isAdmin)
                return false;

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }

        private static ReviewDto ToDto(Review r) => new(
            r.Id, r.ProductId,
            r.User.UserName ?? r.User.Email ?? "Unknown",
            r.Rating,
            r.Comment,
            r.CreatedAt
        );
    }
}
