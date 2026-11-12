using SmartCafe.API.Domain.DTOs.Wallets;

namespace SmartCafe.API.Application.Interfaces;

public interface IWalletService
{
    Task<WalletResponseDto> GetOrCreateWalletAsync(Guid userId);
    Task<IEnumerable<WalletTransactionResponseDto>> GetTransactionsAsync(Guid userId);
    Task<WalletResponseDto> TopUpAsync(Guid userId, TopUpWalletRequestDto request);

    // Hai phương thức nghiệp vụ nội bộ dùng cho OrderService
    Task DeductForOrderAsync(Guid userId, Guid orderId, decimal amount, string description);
    Task RefundForOrderAsync(Guid userId, Guid orderId, decimal amount, string description);
}