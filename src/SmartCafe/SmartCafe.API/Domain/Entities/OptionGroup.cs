namespace SmartCafe.API.Domain.Entities
{
    public class OptionGroup
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string InputType { get; set; } = "Single";
        public bool IsRequired { get; set; } = false;
        public bool AllowMultiple { get; set; } = false;
        public ICollection<Option> Options { get; set; } = new List<Option>();

        public ICollection<ProductOptionGroup> ProductOptionGroups { get; set; } = new List<ProductOptionGroup>();
    }
}
