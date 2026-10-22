using System.Net;

namespace SmartCafe.API.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid? CustomerId { get; set; } 
        public string OrderType { get; set; } = "DineIn"; 
        public int? TableNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Preparing, Completed, Cancelled
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User? Customer { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
