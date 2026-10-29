using Microsoft.EntityFrameworkCore;
using SmartCafe.API.Application.Interfaces;
using SmartCafe.API.Domain.DTOs.Categories;
using SmartCafe.API.Domain.DTOs.Category;
using SmartCafe.API.Domain.Entities;
using SmartCafe.API.Infrastructure.Data;

namespace SmartCafe.API.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly SmartCafeDbContext _context;

    public CategoryService(SmartCafeDbContext context)
    {
        _context = context;
    }

    // 1. Lấy danh sách tất cả các danh mục đang hoạt động (IsActive == true)
    public async Task<IEnumerable<CategoryResponseDto>> GetAllAsync()
    {
        return await _context.Categories
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                DisplayOrder = c.DisplayOrder,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();
    }

    // 2. Lấy thông tin chi tiết 1 danh mục theo ID
    public async Task<CategoryResponseDto?> GetByIdAsync(Guid id)
    {
        var category = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

        if (category == null) return null;

        return new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            DisplayOrder = category.DisplayOrder,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt
        };
    }

    // 3. Tạo mới một danh mục
    public async Task<CategoryResponseDto> CreateAsync(CreateCategoryRequestDto request)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            DisplayOrder = request.DisplayOrder,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            DisplayOrder = category.DisplayOrder,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt
        };
    }

    // 4. Cập nhật thông tin danh mục theo ID
    public async Task<CategoryResponseDto?> UpdateAsync(Guid id, UpdateCategoryRequestDto request)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category == null) return null;

        category.Name = request.Name;
        category.Description = request.Description;
        category.DisplayOrder = request.DisplayOrder;
        category.IsActive = request.IsActive;

        await _context.SaveChangesAsync();

        return new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            DisplayOrder = category.DisplayOrder,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt
        };
    }

    // 5. Xóa mềm (Soft Delete) danh mục theo ID
    public async Task<bool> DeleteAsync(Guid id)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category == null) return false;

        // Đổi trạng thái IsActive = false thay vì xóa khỏi DB
        category.IsActive = false;
        await _context.SaveChangesAsync();

        return true;
    }
}