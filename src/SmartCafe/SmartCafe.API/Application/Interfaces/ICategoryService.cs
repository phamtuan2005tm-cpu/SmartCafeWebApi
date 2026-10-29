using SmartCafe.API.Domain.DTOs.Categories;
using SmartCafe.API.Domain.DTOs.Category;

namespace SmartCafe.API.Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryResponseDto>> GetAllAsync();

    Task<CategoryResponseDto?> GetByIdAsync(Guid id);

    Task<CategoryResponseDto> CreateAsync(CreateCategoryRequestDto request);

    Task<CategoryResponseDto?> UpdateAsync(Guid id, UpdateCategoryRequestDto request);

    Task<bool> DeleteAsync(Guid id);
}