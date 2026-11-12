namespace SmartCafe.API.Domain.DTOs.Tables;

public class TableResponseDto
{
    public Guid Id { get; set; }
    public int TableNumber { get; set; }
    public string TableName { get; set; } = string.Empty;
    public string QrCodeToken { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}