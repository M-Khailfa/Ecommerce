using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ecommerce.Core.Dtos.Reviews
{
    public record CreateReviewDto(
        [Required] int ProductId,
        [Range(1, 5)] int Rating,
        [MaxLength(1000)] string? Comment
    );

    public record ReviewDto(
        int Id,
        int ProductId,
        string UserName,
        int Rating,
        string? Comment,
        DateTime CreatedAt
    );
}
