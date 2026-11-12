using System.ComponentModel.DataAnnotations;

namespace SmartCafe.API.Domain.DTOs.Tables;

public class UpdateTableStatusRequestDto
{
    [Required(ErrorMessage = "Trạng thái không được để trống.")]
    [RegularExpression("^(Available|Occupied|Maintenance)$", ErrorMessage = "Trạng thái chỉ chấp nhận: Available, Occupied, Maintenance.")]
    public string Status { get; set; } = "Available";
}