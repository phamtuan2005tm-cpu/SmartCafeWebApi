using System.ComponentModel.DataAnnotations;

namespace SmartCafe.API.Domain.DTOs.Options;

public class CreateOptionGroupRequestDto
{
    [Required(ErrorMessage = "Tên nhóm tùy chọn không được để trống.")]
    [MaxLength(100, ErrorMessage = "Tên nhóm tùy chọn không được vượt quá 100 ký tự.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "InputType không được để trống.")]
    [MaxLength(50)]
    public string InputType { get; set; } = "Single"; // "Single", "Multiple", "Slider"...

    public bool IsRequired { get; set; } = false;

    public bool AllowMultiple { get; set; } = false;
}