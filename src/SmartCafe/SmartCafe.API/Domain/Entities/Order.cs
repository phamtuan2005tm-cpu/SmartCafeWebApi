namespace SmartCafe.API.Domain.Entities;

public class Order
{
    public Guid Id { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? TableId { get; set; } // Nối khóa ngoại sang bảng Table (Nullable vì đơn Pickup/Delivery không có bàn)

    public string OrderType { get; set; } = "DineIn"; // DineIn (Tại quán), Pickup (Mang đi), Delivery (Giao hàng)
    public decimal TotalAmount { get; set; }

    // Vòng đời trạng thái chuẩn: PendingPayment -> Paid -> Preparing -> Ready -> Completed | Cancelled
    public string Status { get; set; } = "PendingPayment";

    public DateTime? ScheduledPickupTime { get; set; } // Giờ hẹn nhận món (dành cho đơn Pickup)
    public string? Note { get; set; } // Ghi chú đơn hàng (VD: "Lấy thêm ống hút giấy")
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public User? Customer { get; set; }
    public Table? Table { get; set; }
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public decimal SubTotal { get; set; }          // Tổng tiền món + topping trước giảm giá
    public decimal DiscountAmount { get; set; } = 0; // Số tiền được Voucher giảm
    public string? AppliedVoucherCode { get; set; }  // Mã Voucher đã áp dụng (nếu có)
}