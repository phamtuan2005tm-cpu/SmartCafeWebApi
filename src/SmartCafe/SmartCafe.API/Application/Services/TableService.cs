using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartCafe.API.Application.Interfaces;
using SmartCafe.API.Domain.DTOs.Tables;
using SmartCafe.API.Domain.Entities;
using SmartCafe.API.Infrastructure.Data;

namespace SmartCafe.API.Application.Services;

public class TableService : ITableService
{
    private readonly SmartCafeDbContext _context;

    public TableService(SmartCafeDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TableResponseDto>> GetAllAsync()
    {
        return await _context.Tables
            .AsNoTracking()
            .OrderBy(t => t.TableNumber)
            .Select(t => new TableResponseDto
            {
                Id = t.Id,
                TableNumber = t.TableNumber,
                TableName = t.TableName,
                QrCodeToken = t.QrCodeToken,
                Capacity = t.Capacity,
                Status = t.Status,
                IsActive = t.IsActive,
                CreatedAt = t.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<TableResponseDto?> GetByIdAsync(Guid id)
    {
        var table = await _context.Tables.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
        if (table == null) return null;

        return MapToDto(table);
    }

    public async Task<TableResponseDto?> GetByQrTokenAsync(string qrCodeToken)
    {
        var table = await _context.Tables
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.QrCodeToken == qrCodeToken && t.IsActive);

        if (table == null) return null;

        if (table.Status == "Maintenance")
        {
            throw new BadHttpRequestException($"Bàn {table.TableNumber} hiện đang bảo trì, vui lòng chọn bàn khác.");
        }

        return MapToDto(table);
    }

    public async Task<TableResponseDto> CreateAsync(CreateTableRequestDto request)
    {
        var isNumberExist = await _context.Tables.AnyAsync(t => t.TableNumber == request.TableNumber);
        if (isNumberExist)
        {
            throw new BadHttpRequestException($"Số bàn {request.TableNumber} đã tồn tại trong hệ thống.");
        }

        var table = new Table
        {
            Id = Guid.NewGuid(),
            TableNumber = request.TableNumber,
            TableName = request.TableName,
            // Tự động sinh mã QR Token duy nhất, khó đoán mò để tránh spam từ xa
            QrCodeToken = $"SMARTCAFE-TBL-{request.TableNumber:D2}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}",
            Capacity = request.Capacity,
            Status = "Available",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Tables.Add(table);
        await _context.SaveChangesAsync();

        return MapToDto(table);
    }

    public async Task<TableResponseDto?> UpdateAsync(Guid id, UpdateTableRequestDto request)
    {
        var table = await _context.Tables.FindAsync(id);
        if (table == null) return null;

        var isNumberExist = await _context.Tables
            .AnyAsync(t => t.TableNumber == request.TableNumber && t.Id != id);
        if (isNumberExist)
        {
            throw new BadHttpRequestException($"Số bàn {request.TableNumber} đã được sử dụng cho bàn khác.");
        }

        table.TableNumber = request.TableNumber;
        table.TableName = request.TableName;
        table.Capacity = request.Capacity;
        table.Status = request.Status;
        table.IsActive = request.IsActive;

        await _context.SaveChangesAsync();

        return MapToDto(table);
    }

    public async Task<TableResponseDto?> UpdateStatusAsync(Guid id, string status)
    {
        var table = await _context.Tables.FindAsync(id);
        if (table == null) return null;

        table.Status = status;
        await _context.SaveChangesAsync();

        return MapToDto(table);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var table = await _context.Tables.FindAsync(id);
        if (table == null) return false;

        _context.Tables.Remove(table);
        await _context.SaveChangesAsync();
        return true;
    }

    private static TableResponseDto MapToDto(Table t) => new()
    {
        Id = t.Id,
        TableNumber = t.TableNumber,
        TableName = t.TableName,
        QrCodeToken = t.QrCodeToken,
        Capacity = t.Capacity,
        Status = t.Status,
        IsActive = t.IsActive,
        CreatedAt = t.CreatedAt
    };
}