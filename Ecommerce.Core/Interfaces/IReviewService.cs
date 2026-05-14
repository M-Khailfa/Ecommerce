using Ecommerce.Core.Dtos.Reviews;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Core.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDto>> GetByProductAsync(int productId);
        Task<(ReviewDto? Review, string? Error)> CreateAsync(string userId, CreateReviewDto dto);
        Task<bool> DeleteAsync(int id, string userId, bool isAdmin);
    }
}
