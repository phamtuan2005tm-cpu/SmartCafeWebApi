using SmartCafe.API.Domain.DTOs.Products;

namespace SmartCafe.API.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductResponseDto>> GetAllAsync();
    Task<ProductResponseDto?> GetByIdAsync(Guid id);
    Task<ProductResponseDto> CreateAsync(CreateProductRequestDto request);
    Task<ProductResponseDto?> UpdateAsync(Guid id, UpdateProductRequestDto request);
    Task<bool> DeleteAsync(Guid id);
}