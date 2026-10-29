namespace SmartCafe.API.Domain.DTOs.Products;

public class ProductResponseDto
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty; 
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public int StockQuantity { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime CreatedAt { get; set; }
}