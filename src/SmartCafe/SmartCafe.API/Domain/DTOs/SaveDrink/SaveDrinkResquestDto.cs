namespace SmartCafe.API.Domain.DTOs.SavedDrinks;

public class SavedDrinkResponseDto
{
    public Guid Id { get; set; }
    public string CustomName { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductImageUrl { get; set; }
    public decimal BasePrice { get; set; }
    public decimal EstimatedUnitPrice { get; set; } // Giá 1 ly đã cộng tất cả option
    public DateTime CreatedAt { get; set; }

    public List<SavedDrinkOptionDto> Options { get; set; } = new();
}

public class SavedDrinkOptionDto
{
    public Guid OptionId { get; set; }
    public string OptionName { get; set; } = string.Empty;
    public decimal AdditionalPrice { get; set; }
}