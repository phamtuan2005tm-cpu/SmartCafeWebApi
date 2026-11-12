using System.ComponentModel.DataAnnotations;

namespace SmartCafe.API.Domain.DTOs.Options;

public class CreateOptionRequestDto
{
    [Required(ErrorMessage = "OptionGroupId không được để trống.")]
    public Guid OptionGroupId { get; set; }

    [Required(ErrorMessage = "Tên tùy chọn không được để trống.")]
    [MaxLength(100, ErrorMessage = "Tên tùy chọn không được vượt quá 100 ký tự.")]
    public string Name { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "Giá cộng thêm phải lớn hơn hoặc bằng 0.")]
    public decimal AdditionalPrice { get; set; } = 0;

    [Range(0, double.MaxValue, ErrorMessage = "Định lượng phải lớn hơn hoặc bằng 0.")]
    public decimal QuantityValue { get; set; } = 0;

    [MaxLength(20, ErrorMessage = "Đơn vị tính tối đa 20 ký tự.")]
    public string Unit { get; set; } = "ml";

    public string? LayerImageUrl { get; set; }

    public int DisplayOrder { get; set; } = 0;

    public bool IsAvailable { get; set; } = true;
}