using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartCafe.API.Application.Interfaces;
using SmartCafe.API.Domain.DTOs.Products;
using SmartCafe.API.Domain.Entities;
using SmartCafe.API.Infrastructure.Data;

namespace SmartCafe.API.Application.Services;

public class ProductService : IProductService
{
    private readonly SmartCafeDbContext _context;

    public ProductService(SmartCafeDbContext context)
    {
        _context = context;
    }

    // 1. Lấy danh sách tất cả sản phẩm
    public async Task<IEnumerable<ProductResponseDto>> GetAllAsync()
    {
        return await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Select(p => new ProductResponseDto
            {
                Id = p.Id,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                StockQuantity = p.StockQuantity,
                IsAvailable = p.IsAvailable,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();
    }

    // 2. Lấy chi tiết 1 sản phẩm theo ID
    public async Task<ProductResponseDto?> GetByIdAsync(Guid id)
    {
        var product = await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null) return null;

        return new ProductResponseDto
        {
            Id = product.Id,
            CategoryId = product.CategoryId,
            CategoryName = product.Category.Name,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            StockQuantity = product.StockQuantity,
            IsAvailable = product.IsAvailable,
            CreatedAt = product.CreatedAt
        };
    }

    // 3. Tạo mới sản phẩm
    public async Task<ProductResponseDto> CreateAsync(CreateProductRequestDto request)
    {
        // Kiểm tra CategoryId có tồn tại hay không
        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == request.CategoryId && c.IsActive);

        if (!categoryExists)
        {
            throw new BadHttpRequestException($"Không tìm thấy danh mục với ID: {request.CategoryId}");
        }

        // Kiểm tra trùng tên sản phẩm (không phân biệt hoa thường)
        var isNameExist = await _context.Products
            .AnyAsync(p => p.Name.ToLower() == request.Name.ToLower());

        if (isNameExist)
        {
            throw new BadHttpRequestException($"Sản phẩm với tên '{request.Name}' đã tồn tại trong hệ thống.");
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            CategoryId = request.CategoryId,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            ImageUrl = request.ImageUrl,
            StockQuantity = request.StockQuantity,
            IsAvailable = request.IsAvailable,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var category = await _context.Categories.FindAsync(request.CategoryId);

        return new ProductResponseDto
        {
            Id = product.Id,
            CategoryId = product.CategoryId,
            CategoryName = category?.Name ?? string.Empty,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            StockQuantity = product.StockQuantity,
            IsAvailable = product.IsAvailable,
            CreatedAt = product.CreatedAt
        };
    }

    // 4. Cập nhật thông tin sản phẩm
    public async Task<ProductResponseDto?> UpdateAsync(Guid id, UpdateProductRequestDto request)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null) return null;

        // Kiểm tra CategoryId mới có tồn tại không
        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == request.CategoryId && c.IsActive);

        if (!categoryExists)
        {
            throw new BadHttpRequestException($"Không tìm thấy danh mục với ID: {request.CategoryId}");
        }

        // Kiểm tra trùng tên với sản phẩm khác (loại trừ chính nó)
        var isNameExist = await _context.Products
            .AnyAsync(p => p.Name.ToLower() == request.Name.ToLower() && p.Id != id);

        if (isNameExist)
        {
            throw new BadHttpRequestException($"Sản phẩm với tên '{request.Name}' đã tồn tại trong hệ thống.");
        }

        product.CategoryId = request.CategoryId;
        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.ImageUrl = request.ImageUrl;
        product.StockQuantity = request.StockQuantity;
        product.IsAvailable = request.IsAvailable;

        await _context.SaveChangesAsync();

        var category = await _context.Categories.FindAsync(request.CategoryId);

        return new ProductResponseDto
        {
            Id = product.Id,
            CategoryId = product.CategoryId,
            CategoryName = category?.Name ?? string.Empty,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            StockQuantity = product.StockQuantity,
            IsAvailable = product.IsAvailable,
            CreatedAt = product.CreatedAt
        };
    }

    // 5. Xóa sản phẩm
    public async Task<bool> DeleteAsync(Guid id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null) return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return true;
    }
}