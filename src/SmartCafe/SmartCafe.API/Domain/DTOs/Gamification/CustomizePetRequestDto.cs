using System.ComponentModel.DataAnnotations;

namespace SmartCafe.API.Domain.DTOs.Gamification;

public class CustomizePetRequestDto
{
    [Required(ErrorMessage = "Tên thú cưng không được để trống.")]
    [MaxLength(50, ErrorMessage = "Tên thú cưng tối đa 50 ký tự.")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(Capybara|Cat|Dog|Bunny)$", ErrorMessage = "Loài thú cưng chỉ hỗ trợ: Capybara, Cat, Dog, Bunny.")]
    public string PetType { get; set; } = "Capybara";
}