using SmartCafe.API.Domain.DTOs.Options;

namespace SmartCafe.API.Application.Interfaces;

public interface IOptionGroupService
{
    Task<IEnumerable<OptionGroupResponseDto>> GetAllAsync();
    Task<OptionGroupResponseDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<OptionGroupResponseDto>> GetByProductIdAsync(Guid productId);
    Task<OptionGroupResponseDto> CreateAsync(CreateOptionGroupRequestDto request);
    Task<OptionGroupResponseDto?> UpdateAsync(Guid id, UpdateOptionGroupRequestDto request);
    Task<bool> DeleteAsync(Guid id);
    Task AssignToProductAsync(Guid productId, AssignOptionGroupsRequestDto request);
}