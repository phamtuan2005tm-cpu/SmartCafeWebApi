namespace SmartCafe.API.Domain.DTOs.Wallets;

public class WalletResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal Balance { get; set; }
    public bool IsLocked { get; set; }
    public DateTime UpdatedAt { get; set; }
}