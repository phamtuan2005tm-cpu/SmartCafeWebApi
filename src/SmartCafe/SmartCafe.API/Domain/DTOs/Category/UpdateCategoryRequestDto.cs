using System.ComponentModel.DataAnnotations;

namespace SmartCafe.API.Domain.DTOs.Categories;

public class UpdateCategoryRequestDto
{
    [Required(ErrorMessage = "Tên danh mục không được để trống.")]
    [MaxLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự.")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
    public string? Description { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Thứ tự hiển thị phải lớn hơn hoặc bằng 0.")]
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}