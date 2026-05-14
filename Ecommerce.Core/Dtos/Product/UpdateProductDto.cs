using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ecommerce.Core.Dtos.Product
{
    public record UpdateProductDto(
        [Required, MaxLength(200)] string Name,
        [Required] int CategoryId,
        [MaxLength(100)] string? Brand,
        string? Description,
        decimal Price,
        int Stock,
        IFormFile? Image,
        bool IsActive
    );
}
