using System.ComponentModel.DataAnnotations;

namespace SmartCafe.API.Domain.DTOs.SavedDrinks;

public class OrderSavedDrinkRequestDto
{
    [Required]
    [RegularExpression("^(DineIn|Pickup|Delivery)$", ErrorMessage = "Loại đơn chỉ chấp nhận: DineIn, Pickup, Delivery.")]
    public string OrderType { get; set; } = "DineIn";

    public Guid? TableId { get; set; }

    [Range(1, 50, ErrorMessage = "Số lượng phải từ 1 đến 50.")]
    public int Quantity { get; set; } = 1;

    public DateTime? ScheduledPickupTime { get; set; }

    [MaxLength(500)]
    public string? Note { get; set; }
}