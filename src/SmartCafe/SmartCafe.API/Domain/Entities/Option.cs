namespace SmartCafe.API.Domain.Entities
{
    public class Option
    {
        public Guid Id { get; set; }
        public Guid OptionGroupId { get; set; } 
        public string Name { get; set; } = string.Empty;
        public decimal AdditionalPrice { get; set; } = 0; 
        public decimal QuantityValue { get; set; } = 0;   
        public string Unit { get; set; } = "ml";           
        public string? LayerImageUrl { get; set; }        
        public int DisplayOrder { get; set; } = 0;
        public decimal ExtraPrice { get; set; } = 0;
        public bool IsAvailable { get; set; } = true;

        public OptionGroup OptionGroup { get; set; } = null!;

        public ICollection<OrderDetailOption> OrderDetailOptions { get; set; } = new List<OrderDetailOption>();
    }
}
