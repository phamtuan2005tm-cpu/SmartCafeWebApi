using System.ComponentModel.DataAnnotations;

namespace SmartCafe.API.Domain.DTOs.Options;

public class AssignOptionGroupsRequestDto
{
    [Required(ErrorMessage = "Danh sách OptionGroupIds không được để trống.")]
    public List<Guid> OptionGroupIds { get; set; } = new();
}