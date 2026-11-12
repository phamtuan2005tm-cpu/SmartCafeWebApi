namespace SmartCafe.API.Domain.Entities;

public class WalletTransaction
{
    public Guid Id { get; set; }
    public Guid WalletId { get; set; }
    public Guid? OrderId { get; set; } // Nullable: TopUp không gắn với Order, Payment/Refund gắn với Order

    public decimal Amount { get; set; }
    public decimal BalanceBefore { get; set; }
    public decimal BalanceAfter { get; set; }

    public string Type { get; set; } = string.Empty; // "TopUp", "Payment", "Refund"
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Wallet Wallet { get; set; } = null!;
    public Order? Order { get; set; }
}