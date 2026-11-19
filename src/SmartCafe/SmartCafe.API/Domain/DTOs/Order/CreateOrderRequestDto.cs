using System.ComponentModel.DataAnnotations;

namespace SmartCafe.API.Domain.DTOs.Orders;

public class CreateOrderRequestDto
{
    [Required(ErrorMessage = "Loại đơn hàng không được để trống.")]
    [RegularExpression("^(DineIn|Pickup|Delivery)$", ErrorMessage = "Loại đơn chỉ chấp nhận: DineIn, Pickup, Delivery.")]
    public string OrderType { get; set; } = "DineIn";

    public Guid? TableId { get; set; } // Bắt buộc nếu OrderType == "DineIn"
    public string? VoucherCode { get; set; }
    public DateTime? ScheduledPickupTime { get; set; } // Tùy chọn cho đơn Pickup

    [MaxLength(500)]
    public string? Note { get; set; }

    [Required(ErrorMessage = "Danh sách món không được để trống.")]
    [MinLength(1, ErrorMessage = "Đơn hàng phải có ít nhất 1 món.")]
    public List<CreateOrderItemDto> Items { get; set; } = new();
}

public class CreateOrderItemDto
{
    [Required]
    public Guid ProductId { get; set; }

    [Range(1, 50, ErrorMessage = "Số lượng mỗi món phải từ 1 đến 50.")]
    public int Quantity { get; set; } = 1;

    // Danh sách ID của các Option (Size, Đường, Đá, Topping...) mà khách đã chọn cho món này
    public List<Guid> SelectedOptionIds { get; set; } = new();
}