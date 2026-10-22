namespace SmartCafe.API.Domain.Entities
{
    public class OrderDetailOption
    {
        public Guid Id { get; set; }
        public Guid OrderDetailId { get; set; }
        public Guid OptionId { get; set; }     

        public string OptionNameSnapshot { get; set; } = string.Empty;
        public decimal AdditionalPriceSnapshot { get; set; } = 0;

        public OrderDetail OrderDetail { get; set; } = null!;
        public Option Option { get; set; } = null!;
    }
}
