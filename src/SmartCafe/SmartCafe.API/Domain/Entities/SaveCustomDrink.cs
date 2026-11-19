namespace SmartCafe.API.Domain.Entities;

public class SavedCustomDrink
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }

    public string CustomName { get; set; } = string.Empty; // VD: "Trà sữa ít ngọt gu của Tuấn"
    public string SelectedOptionIdsJson { get; set; } = "[]"; // Lưu mảng JSON các OptionId đã chọn
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User User { get; set; } = null!;
    public Product Product { get; set; } = null!;
}