namespace SmartCafe.API.Domain.Entities;

public class Voucher
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty; // VD: "PETLV2-8F9A", "WELCOME20"
    public string Title { get; set; } = string.Empty; // VD: "Quà mừng Pet đạt Level 2"
    public string? Description { get; set; }

    // Null = Voucher công khai toàn quán; Có giá trị = Voucher độc quyền của User đó
    public Guid? UserId { get; set; }

    public string DiscountType { get; set; } = "FixedAmount"; // "FixedAmount" | "Percentage"
    public decimal DiscountValue { get; set; } // Số tiền giảm (VNĐ) hoặc % giảm (VD: 15 hoặc 20000)
    public decimal? MaxDiscountAmount { get; set; } // Trần giảm tối đa (bắt buộc nếu là Percentage)
    public decimal MinOrderAmount { get; set; } = 0; // Giá trị đơn tối thiểu để áp dụng

    public int UsageLimit { get; set; } = 1; // Tổng số lượt được dùng của mã này (Voucher cá nhân = 1)
    public int UsedCount { get; set; } = 0;  // Số lượt đã sử dụng thực tế

    public DateTime ValidFrom { get; set; } = DateTime.UtcNow;
    public DateTime ValidTo { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User? User { get; set; }
    public ICollection<VoucherUsage> Usages { get; set; } = new List<VoucherUsage>();
}