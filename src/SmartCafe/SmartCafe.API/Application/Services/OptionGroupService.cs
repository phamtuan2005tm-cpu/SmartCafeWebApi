using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartCafe.API.Application.Interfaces;
using SmartCafe.API.Domain.DTOs.Options;
using SmartCafe.API.Domain.Entities;
using SmartCafe.API.Infrastructure.Data;

namespace SmartCafe.API.Application.Services;

public class OptionGroupService : IOptionGroupService
{
    private readonly SmartCafeDbContext _context;

    public OptionGroupService(SmartCafeDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OptionGroupResponseDto>> GetAllAsync()
    {
        return await _context.OptionGroups
            .AsNoTracking()
            .Include(og => og.Options)
            .Select(og => new OptionGroupResponseDto
            {
                Id = og.Id,
                Name = og.Name,
                InputType = og.InputType,
                IsRequired = og.IsRequired,
                AllowMultiple = og.AllowMultiple,
                Options = og.Options
                    .OrderBy(o => o.DisplayOrder)
                    .Select(o => new OptionResponseDto
                    {
                        Id = o.Id,
                        OptionGroupId = o.OptionGroupId,
                        OptionGroupName = og.Name,
                        Name = o.Name,
                        AdditionalPrice = o.AdditionalPrice,
                        QuantityValue = o.QuantityValue,
                        Unit = o.Unit,
                        LayerImageUrl = o.LayerImageUrl,
                        DisplayOrder = o.DisplayOrder,
                        IsAvailable = o.IsAvailable
                    }).ToList()
            })
            .ToListAsync();
    }

    public async Task<OptionGroupResponseDto?> GetByIdAsync(Guid id)
    {
        var group = await _context.OptionGroups
            .AsNoTracking()
            .Include(og => og.Options)
            .FirstOrDefaultAsync(og => og.Id == id);

        if (group == null) return null;

        return new OptionGroupResponseDto
        {
            Id = group.Id,
            Name = group.Name,
            InputType = group.InputType,
            IsRequired = group.IsRequired,
            AllowMultiple = group.AllowMultiple,
            Options = group.Options
                .OrderBy(o => o.DisplayOrder)
                .Select(o => new OptionResponseDto
                {
                    Id = o.Id,
                    OptionGroupId = o.OptionGroupId,
                    OptionGroupName = group.Name,
                    Name = o.Name,
                    AdditionalPrice = o.AdditionalPrice,
                    QuantityValue = o.QuantityValue,
                    Unit = o.Unit,
                    LayerImageUrl = o.LayerImageUrl,
                    DisplayOrder = o.DisplayOrder,
                    IsAvailable = o.IsAvailable
                }).ToList()
        };
    }

    public async Task<IEnumerable<OptionGroupResponseDto>> GetByProductIdAsync(Guid productId)
    {
        var productExists = await _context.Products.AnyAsync(p => p.Id == productId);
        if (!productExists)
        {
            throw new BadHttpRequestException($"Không tìm thấy sản phẩm với ID: {productId}");
        }

        return await _context.ProductOptionGroups
            .AsNoTracking()
            .Where(pog => pog.ProductId == productId)
            .Include(pog => pog.OptionGroup)
                .ThenInclude(og => og.Options)
            .Select(pog => new OptionGroupResponseDto
            {
                Id = pog.OptionGroup.Id,
                Name = pog.OptionGroup.Name,
                InputType = pog.OptionGroup.InputType,
                IsRequired = pog.OptionGroup.IsRequired,
                AllowMultiple = pog.OptionGroup.AllowMultiple,
                Options = pog.OptionGroup.Options
                    .Where(o => o.IsAvailable)
                    .OrderBy(o => o.DisplayOrder)
                    .Select(o => new OptionResponseDto
                    {
                        Id = o.Id,
                        OptionGroupId = o.OptionGroupId,
                        OptionGroupName = pog.OptionGroup.Name,
                        Name = o.Name,
                        AdditionalPrice = o.AdditionalPrice,
                        QuantityValue = o.QuantityValue,
                        Unit = o.Unit,
                        LayerImageUrl = o.LayerImageUrl,
                        DisplayOrder = o.DisplayOrder,
                        IsAvailable = o.IsAvailable
                    }).ToList()
            })
            .ToListAsync();
    }

    public async Task<OptionGroupResponseDto> CreateAsync(CreateOptionGroupRequestDto request)
    {
        var isNameExist = await _context.OptionGroups
            .AnyAsync(og => og.Name.ToLower() == request.Name.ToLower());

        if (isNameExist)
        {
            throw new BadHttpRequestException($"Nhóm tùy chọn '{request.Name}' đã tồn tại.");
        }

        var group = new OptionGroup
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            InputType = request.InputType,
            IsRequired = request.IsRequired,
            AllowMultiple = request.AllowMultiple
        };

        _context.OptionGroups.Add(group);
        await _context.SaveChangesAsync();

        return new OptionGroupResponseDto
        {
            Id = group.Id,
            Name = group.Name,
            InputType = group.InputType,
            IsRequired = group.IsRequired,
            AllowMultiple = group.AllowMultiple,
            Options = new List<OptionResponseDto>()
        };
    }

    public async Task<OptionGroupResponseDto?> UpdateAsync(Guid id, UpdateOptionGroupRequestDto request)
    {
        var group = await _context.OptionGroups
            .Include(og => og.Options)
            .FirstOrDefaultAsync(og => og.Id == id);

        if (group == null) return null;

        var isNameExist = await _context.OptionGroups
            .AnyAsync(og => og.Name.ToLower() == request.Name.ToLower() && og.Id != id);

        if (isNameExist)
        {
            throw new BadHttpRequestException($"Nhóm tùy chọn '{request.Name}' đã tồn tại.");
        }

        group.Name = request.Name;
        group.InputType = request.InputType;
        group.IsRequired = request.IsRequired;
        group.AllowMultiple = request.AllowMultiple;

        await _context.SaveChangesAsync();

        return new OptionGroupResponseDto
        {
            Id = group.Id,
            Name = group.Name,
            InputType = group.InputType,
            IsRequired = group.IsRequired,
            AllowMultiple = group.AllowMultiple,
            Options = group.Options
                .OrderBy(o => o.DisplayOrder)
                .Select(o => new OptionResponseDto
                {
                    Id = o.Id,
                    OptionGroupId = o.OptionGroupId,
                    OptionGroupName = group.Name,
                    Name = o.Name,
                    AdditionalPrice = o.AdditionalPrice,
                    QuantityValue = o.QuantityValue,
                    Unit = o.Unit,
                    LayerImageUrl = o.LayerImageUrl,
                    DisplayOrder = o.DisplayOrder,
                    IsAvailable = o.IsAvailable
                }).ToList()
        };
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var group = await _context.OptionGroups.FindAsync(id);
        if (group == null) return false;

        _context.OptionGroups.Remove(group);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task AssignToProductAsync(Guid productId, AssignOptionGroupsRequestDto request)
    {
        var productExists = await _context.Products.AnyAsync(p => p.Id == productId);
        if (!productExists)
        {
            throw new BadHttpRequestException($"Không tìm thấy sản phẩm với ID: {productId}");
        }

        var distinctGroupIds = request.OptionGroupIds.Distinct().ToList();

        var validGroupCount = await _context.OptionGroups
            .CountAsync(og => distinctGroupIds.Contains(og.Id));

        if (validGroupCount != distinctGroupIds.Count)
        {
            throw new BadHttpRequestException("Một hoặc nhiều OptionGroupId không tồn tại trong hệ thống.");
        }

        // Xóa cấu hình nhóm cũ của sản phẩm và thay thế bằng danh sách mới
        var existingLinks = await _context.ProductOptionGroups
            .Where(pog => pog.ProductId == productId)
            .ToListAsync();

        _context.ProductOptionGroups.RemoveRange(existingLinks);

        var newLinks = distinctGroupIds.Select(groupId => new ProductOptionGroup
        {
            ProductId = productId,
            OptionGroupId = groupId
        });

        await _context.ProductOptionGroups.AddRangeAsync(newLinks);
        await _context.SaveChangesAsync();
    }
}