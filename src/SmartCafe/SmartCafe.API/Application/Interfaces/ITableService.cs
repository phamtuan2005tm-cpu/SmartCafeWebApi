using SmartCafe.API.Domain.DTOs.Tables;

namespace SmartCafe.API.Application.Interfaces;

public interface ITableService
{
    Task<IEnumerable<TableResponseDto>> GetAllAsync();
    Task<TableResponseDto?> GetByIdAsync(Guid id);
    Task<TableResponseDto?> GetByQrTokenAsync(string qrCodeToken);
    Task<TableResponseDto> CreateAsync(CreateTableRequestDto request);
    Task<TableResponseDto?> UpdateAsync(Guid id, UpdateTableRequestDto request);
    Task<TableResponseDto?> UpdateStatusAsync(Guid id, string status);
    Task<bool> DeleteAsync(Guid id);
}