using Ecommerce.Core.Dtos.Product;
using Ecommerce.Core.Settings;

namespace Ecommerce.Core.Interfaces
{
    public interface IProductService
    {
        Task<PagedList<ProductDto>> GetAllAsync(
                    string? search, int? categoryId, decimal? minPrice, decimal? maxPrice,
                    int pageNumber = 1, int pageSize = 10); Task<ProductDto?> GetByIdAsync(int id);
        Task<ProductDto> CreateAsync(CreateProductDto dto);
        Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
