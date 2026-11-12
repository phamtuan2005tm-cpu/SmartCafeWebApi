namespace SmartCafe.API.Domain.Entities;

public class Table
{
    public Guid Id { get; set; }
    public int TableNumber { get; set; } // Số bàn: 1, 2, 3...
    public string TableName { get; set; } = string.Empty; // VD: "Bàn 01 - Tầng 1", "Bàn Ban Công"
    public string QrCodeToken { get; set; } = string.Empty; // Chuỗi token duy nhất nhúng vào URL QR
    public int Capacity { get; set; } = 4; // Sức chứa số người
    public string Status { get; set; } = "Available"; // Available (Trống), Occupied (Đang có khách), Maintenance (Bảo trì)
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}