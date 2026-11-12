namespace SmartCafe.API.Domain.DTOs.Options;

public class OptionResponseDto
{
    public Guid Id { get; set; }
    public Guid OptionGroupId { get; set; }
    public string OptionGroupName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal AdditionalPrice { get; set; }
    public decimal QuantityValue { get; set; }
    public string Unit { get; set; } = "ml";
    public string? LayerImageUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsAvailable { get; set; }
}