namespace SmartCafe.API.Domain.Entities;

public class VoucherUsage
{
    public Guid Id { get; set; }
    public Guid VoucherId { get; set; }
    public Guid UserId { get; set; }
    public Guid OrderId { get; set; }
    public decimal DiscountApplied { get; set; } // Số tiền thực tế đã giảm trên đơn này
    public DateTime UsedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Voucher Voucher { get; set; } = null!;
    public User User { get; set; } = null!;
    public Order Order { get; set; } = null!;
}