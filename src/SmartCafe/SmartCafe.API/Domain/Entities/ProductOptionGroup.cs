namespace SmartCafe.API.Domain.Entities
{
    public class ProductOptionGroup
    {
        public Guid ProductId { get; set; }
        public Guid OptionGroupId { get; set; }

        public Product Product { get; set; } = null!;
        public OptionGroup OptionGroup { get; set; } = null!;
    }
}
