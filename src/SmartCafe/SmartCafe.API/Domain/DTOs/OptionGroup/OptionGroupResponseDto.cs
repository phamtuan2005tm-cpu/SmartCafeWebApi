namespace SmartCafe.API.Domain.DTOs.Options;

public class OptionGroupResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string InputType { get; set; } = "Single";
    public bool IsRequired { get; set; }
    public bool AllowMultiple { get; set; }
    public List<OptionResponseDto> Options { get; set; } = new();
}