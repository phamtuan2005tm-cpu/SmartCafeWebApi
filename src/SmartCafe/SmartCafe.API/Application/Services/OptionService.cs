using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartCafe.API.Application.Interfaces;
using SmartCafe.API.Domain.DTOs.Options;
using SmartCafe.API.Domain.Entities;
using SmartCafe.API.Infrastructure.Data;

namespace SmartCafe.API.Application.Services;

public class OptionService : IOptionService
{
    private readonly SmartCafeDbContext _context;

    public OptionService(SmartCafeDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OptionResponseDto>> GetByGroupIdAsync(Guid optionGroupId)
    {
        return await _context.Options
            .AsNoTracking()
            .Include(o => o.OptionGroup)
            .Where(o => o.OptionGroupId == optionGroupId)
            .OrderBy(o => o.DisplayOrder)
            .Select(o => new OptionResponseDto
            {
                Id = o.Id,
                OptionGroupId = o.OptionGroupId,
                OptionGroupName = o.OptionGroup.Name,
                Name = o.Name,
                AdditionalPrice = o.AdditionalPrice,
                QuantityValue = o.QuantityValue,
                Unit = o.Unit,
                LayerImageUrl = o.LayerImageUrl,
                DisplayOrder = o.DisplayOrder,
                IsAvailable = o.IsAvailable
            })
            .ToListAsync();
    }

    public async Task<OptionResponseDto?> GetByIdAsync(Guid id)
    {
        var option = await _context.Options
            .AsNoTracking()
            .Include(o => o.OptionGroup)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (option == null) return null;

        return new OptionResponseDto
        {
            Id = option.Id,
            OptionGroupId = option.OptionGroupId,
            OptionGroupName = option.OptionGroup.Name,
            Name = option.Name,
            AdditionalPrice = option.AdditionalPrice,
            QuantityValue = option.QuantityValue,
            Unit = option.Unit,
            LayerImageUrl = option.LayerImageUrl,
            DisplayOrder = option.DisplayOrder,
            IsAvailable = option.IsAvailable
        };
    }

    public async Task<OptionResponseDto> CreateAsync(CreateOptionRequestDto request)
    {
        var group = await _context.OptionGroups.FindAsync(request.OptionGroupId);
        if (group == null)
        {
            throw new BadHttpRequestException($"Không tìm thấy nhóm tùy chọn với ID: {request.OptionGroupId}");
        }

        // Chặn trùng tên option trong cùng 1 nhóm
        var isNameExist = await _context.Options
            .AnyAsync(o => o.OptionGroupId == request.OptionGroupId && o.Name.ToLower() == request.Name.ToLower());

        if (isNameExist)
        {
            throw new BadHttpRequestException($"Tùy chọn '{request.Name}' đã tồn tại trong nhóm '{group.Name}'.");
        }

        var option = new Option
        {
            Id = Guid.NewGuid(),
            OptionGroupId = request.OptionGroupId,
            Name = request.Name,
            AdditionalPrice = request.AdditionalPrice,
            ExtraPrice = request.AdditionalPrice, // Đồng bộ cả 2 trường giá trên Entity
            QuantityValue = request.QuantityValue,
            Unit = request.Unit,
            LayerImageUrl = request.LayerImageUrl,
            DisplayOrder = request.DisplayOrder,
            IsAvailable = request.IsAvailable
        };

        _context.Options.Add(option);
        await _context.SaveChangesAsync();

        return new OptionResponseDto
        {
            Id = option.Id,
            OptionGroupId = option.OptionGroupId,
            OptionGroupName = group.Name,
            Name = option.Name,
            AdditionalPrice = option.AdditionalPrice,
            QuantityValue = option.QuantityValue,
            Unit = option.Unit,
            LayerImageUrl = option.LayerImageUrl,
            DisplayOrder = option.DisplayOrder,
            IsAvailable = option.IsAvailable
        };
    }

    public async Task<OptionResponseDto?> UpdateAsync(Guid id, UpdateOptionRequestDto request)
    {
        var option = await _context.Options.FindAsync(id);
        if (option == null) return null;

        var group = await _context.OptionGroups.FindAsync(request.OptionGroupId);
        if (group == null)
        {
            throw new BadHttpRequestException($"Không tìm thấy nhóm tùy chọn với ID: {request.OptionGroupId}");
        }

        var isNameExist = await _context.Options
            .AnyAsync(o => o.OptionGroupId == request.OptionGroupId
                        && o.Name.ToLower() == request.Name.ToLower()
                        && o.Id != id);

        if (isNameExist)
        {
            throw new BadHttpRequestException($"Tùy chọn '{request.Name}' đã tồn tại trong nhóm '{group.Name}'.");
        }

        option.OptionGroupId = request.OptionGroupId;
        option.Name = request.Name;
        option.AdditionalPrice = request.AdditionalPrice;
        option.ExtraPrice = request.AdditionalPrice;
        option.QuantityValue = request.QuantityValue;
        option.Unit = request.Unit;
        option.LayerImageUrl = request.LayerImageUrl;
        option.DisplayOrder = request.DisplayOrder;
        option.IsAvailable = request.IsAvailable;

        await _context.SaveChangesAsync();

        return new OptionResponseDto
        {
            Id = option.Id,
            OptionGroupId = option.OptionGroupId,
            OptionGroupName = group.Name,
            Name = option.Name,
            AdditionalPrice = option.AdditionalPrice,
            QuantityValue = option.QuantityValue,
            Unit = option.Unit,
            LayerImageUrl = option.LayerImageUrl,
            DisplayOrder = option.DisplayOrder,
            IsAvailable = option.IsAvailable
        };
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var option = await _context.Options.FindAsync(id);
        if (option == null) return false;

        _context.Options.Remove(option);
        await _context.SaveChangesAsync();
        return true;
    }
}