using SmartCafe.API.Domain.DTOs.Orders;

namespace SmartCafe.API.Application.Interfaces;

public interface IOrderService
{
    Task<OrderResponseDto> CreateOrderAsync(Guid customerId, CreateOrderRequestDto request);
    Task<OrderResponseDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<OrderResponseDto>> GetMyOrdersAsync(Guid customerId);
    Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync(string? status = null);
    Task<OrderResponseDto?> UpdateStatusAsync(Guid orderId, string newStatus);
    Task<OrderResponseDto?> CancelMyOrderAsync(Guid customerId, Guid orderId);
}