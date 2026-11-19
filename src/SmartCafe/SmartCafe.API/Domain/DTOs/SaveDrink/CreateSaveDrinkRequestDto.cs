using System.ComponentModel.DataAnnotations;

namespace SmartCafe.API.Domain.DTOs.SavedDrinks;

public class CreateSavedDrinkRequestDto
{
    [Required(ErrorMessage = "Vui lòng chọn món nước gốc.")]
    public Guid ProductId { get; set; }

    [Required(ErrorMessage = "Tên gọi công thức không được để trống.")]
    [MaxLength(150, ErrorMessage = "Tên công thức tối đa 150 ký tự.")]
    public string CustomName { get; set; } = string.Empty;

    public List<Guid> SelectedOptionIds { get; set; } = new();
}