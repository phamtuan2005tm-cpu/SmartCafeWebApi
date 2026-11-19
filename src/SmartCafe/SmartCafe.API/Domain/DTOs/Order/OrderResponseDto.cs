namespace SmartCafe.API.Domain.DTOs.Orders;

public class OrderResponseDto
{
    public Guid Id { get; set; }
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public Guid? TableId { get; set; }
    public int? TableNumber { get; set; }
    public string? TableName { get; set; }
    public string OrderType { get; set; } = string.Empty;
    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public string? AppliedVoucherCode { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? ScheduledPickupTime { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public List<OrderDetailResponseDto> Items { get; set; } = new();
}

public class OrderDetailResponseDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductImageUrl { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; } // Giá gốc 1 ly (chưa cộng topping)
    public decimal SubTotal { get; set; }  // (UnitPrice + Tổng AdditionalPrice) * Quantity

    public List<OrderDetailOptionResponseDto> Options { get; set; } = new();
}

public class OrderDetailOptionResponseDto
{
    public Guid OptionId { get; set; }
    public string OptionName { get; set; } = string.Empty;
    public decimal AdditionalPrice { get; set; }
    
}