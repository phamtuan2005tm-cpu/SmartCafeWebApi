using SmartCafe.API.Domain.DTOs.Options;

namespace SmartCafe.API.Application.Interfaces;

public interface IOptionService
{
    Task<IEnumerable<OptionResponseDto>> GetByGroupIdAsync(Guid optionGroupId);
    Task<OptionResponseDto?> GetByIdAsync(Guid id);
    Task<OptionResponseDto> CreateAsync(CreateOptionRequestDto request);
    Task<OptionResponseDto?> UpdateAsync(Guid id, UpdateOptionRequestDto request);
    Task<bool> DeleteAsync(Guid id);
}