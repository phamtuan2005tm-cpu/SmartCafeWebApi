namespace SmartCafe.API.Domain.DTOs.Wallets;

public class WalletTransactionResponseDto
{
    public Guid Id { get; set; }
    public Guid? OrderId { get; set; }
    public decimal Amount { get; set; }
    public decimal BalanceBefore { get; set; }
    public decimal BalanceAfter { get; set; }
    public string Type { get; set; } = string.Empty; // TopUp, Payment, Refund
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}