using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Core.Dtos.Product
{
    public record ProductDto(
        int Id,
        string Name,
        string? Brand,
        string? Description,
        decimal Price,
        int Stock,
        string? ImageUrl,
        bool IsActive,
        int CategoryId,
        string CategoryName
    );
}
