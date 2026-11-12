using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartCafe.API.Application.Interfaces;
using SmartCafe.API.Domain.DTOs.Wallets;
using SmartCafe.API.Domain.Entities;
using SmartCafe.API.Infrastructure.Data;

namespace SmartCafe.API.Application.Services;

public class WalletService : IWalletService
{
    private readonly SmartCafeDbContext _context;

    public WalletService(SmartCafeDbContext context)
    {
        _context = context;
    }

    public async Task<WalletResponseDto> GetOrCreateWalletAsync(Guid userId)
    {
        var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);

        if (wallet == null)
        {
            wallet = new Wallet
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Balance = 0,
                IsLocked = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();
        }

        return MapToDto(wallet);
    }

    public async Task<IEnumerable<WalletTransactionResponseDto>> GetTransactionsAsync(Guid userId)
    {
        var wallet = await _context.Wallets
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.UserId == userId);

        if (wallet == null)
        {
            return Enumerable.Empty<WalletTransactionResponseDto>();
        }

        return await _context.WalletTransactions
            .AsNoTracking()
            .Where(wt => wt.WalletId == wallet.Id)
            .OrderByDescending(wt => wt.CreatedAt)
            .Select(wt => new WalletTransactionResponseDto
            {
                Id = wt.Id,
                OrderId = wt.OrderId,
                Amount = wt.Amount,
                BalanceBefore = wt.BalanceBefore,
                BalanceAfter = wt.BalanceAfter,
                Type = wt.Type,
                Description = wt.Description,
                CreatedAt = wt.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<WalletResponseDto> TopUpAsync(Guid userId, TopUpWalletRequestDto request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
            if (wallet == null)
            {
                wallet = new Wallet
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Balance = 0,
                    IsLocked = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Wallets.Add(wallet);
            }

            if (wallet.IsLocked)
            {
                throw new BadHttpRequestException("Ví của bạn hiện đang bị khóa, không thể thực hiện giao dịch.");
            }

            var balanceBefore = wallet.Balance;
            wallet.Balance += request.Amount;
            wallet.UpdatedAt = DateTime.UtcNow;

            var walletTx = new WalletTransaction
            {
                Id = Guid.NewGuid(),
                WalletId = wallet.Id,
                OrderId = null,
                Amount = request.Amount,
                BalanceBefore = balanceBefore,
                BalanceAfter = wallet.Balance,
                Type = "TopUp",
                Description = string.IsNullOrWhiteSpace(request.Note)
                    ? $"Nạp tiền vào ví SmartCafe (+{request.Amount:N0}đ)"
                    : request.Note,
                CreatedAt = DateTime.UtcNow
            };

            _context.WalletTransactions.Add(walletTx);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return MapToDto(wallet);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task DeductForOrderAsync(Guid userId, Guid orderId, decimal amount, string description)
    {
        if (amount <= 0)
            throw new BadHttpRequestException("Số tiền thanh toán phải lớn hơn 0.");

        var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
        if (wallet == null)
            throw new BadHttpRequestException("Bạn chưa kích hoạt ví hoặc số dư ví bằng 0đ.");

        if (wallet.IsLocked)
            throw new BadHttpRequestException("Ví của bạn đang bị khóa.");

        if (wallet.Balance < amount)
            throw new BadHttpRequestException($"Số dư ví không đủ (Hiện có: {wallet.Balance:N0}đ, Cần thanh toán: {amount:N0}đ).");

        var balanceBefore = wallet.Balance;
        wallet.Balance -= amount;
        wallet.UpdatedAt = DateTime.UtcNow;

        var walletTx = new WalletTransaction
        {
            Id = Guid.NewGuid(),
            WalletId = wallet.Id,
            OrderId = orderId,
            Amount = amount,
            BalanceBefore = balanceBefore,
            BalanceAfter = wallet.Balance,
            Type = "Payment",
            Description = description,
            CreatedAt = DateTime.UtcNow
        };

        _context.WalletTransactions.Add(walletTx);
        // Lưu ý: Không gọi SaveChangesAsync/Commit ở đây nếu OrderService đang mở Transaction cha, 
        // hoặc có thể gọi SaveChangesAsync() để EF track cùng transaction của DbContext.
        await _context.SaveChangesAsync();
    }

    public async Task RefundForOrderAsync(Guid userId, Guid orderId, decimal amount, string description)
    {
        if (amount <= 0)
            throw new BadHttpRequestException("Số tiền hoàn phải lớn hơn 0.");

        var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
        if (wallet == null)
        {
            wallet = new Wallet
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Balance = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Wallets.Add(wallet);
        }

        var balanceBefore = wallet.Balance;
        wallet.Balance += amount;
        wallet.UpdatedAt = DateTime.UtcNow;

        var walletTx = new WalletTransaction
        {
            Id = Guid.NewGuid(),
            WalletId = wallet.Id,
            OrderId = orderId,
            Amount = amount,
            BalanceBefore = balanceBefore,
            BalanceAfter = wallet.Balance,
            Type = "Refund",
            Description = description,
            CreatedAt = DateTime.UtcNow
        };

        _context.WalletTransactions.Add(walletTx);
        await _context.SaveChangesAsync();
    }

    private static WalletResponseDto MapToDto(Wallet w) => new()
    {
        Id = w.Id,
        UserId = w.UserId,
        Balance = w.Balance,
        IsLocked = w.IsLocked,
        UpdatedAt = w.UpdatedAt
    };
}