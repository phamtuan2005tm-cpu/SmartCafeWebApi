using System.ComponentModel.DataAnnotations;

namespace SmartCafe.API.Domain.DTOs.Orders;

public class UpdateOrderStatusRequestDto
{
    [Required(ErrorMessage = "Trạng thái đơn hàng không được để trống.")]
    [RegularExpression("^(Preparing|Ready|Completed|Cancelled)$", ErrorMessage = "Trạng thái chỉ chấp nhận: Preparing, Ready, Completed, Cancelled.")]
    public string Status { get; set; } = string.Empty;
}