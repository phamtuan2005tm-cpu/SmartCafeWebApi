using System.ComponentModel.DataAnnotations;

namespace SmartCafe.API.Domain.DTOs.Gamification;

public class CreateVoucherRequestDto
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    [RegularExpression("^(FixedAmount|Percentage)$", ErrorMessage = "Loại giảm giá chỉ chấp nhận: FixedAmount hoặc Percentage.")]
    public string DiscountType { get; set; } = "FixedAmount";

    [Range(1, 10000000)]
    public decimal DiscountValue { get; set; }

    public decimal? MaxDiscountAmount { get; set; }

    [Range(0, 100000000)]
    public decimal MinOrderAmount { get; set; } = 0;

    [Range(1, 100000)]
    public int UsageLimit { get; set; } = 100;

    public DateTime ValidFrom { get; set; } = DateTime.UtcNow;
    public DateTime ValidTo { get; set; } = DateTime.UtcNow.AddDays(30);
}