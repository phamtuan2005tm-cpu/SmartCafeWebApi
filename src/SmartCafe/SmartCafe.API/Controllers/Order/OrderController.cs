using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCafe.API.Application.Interfaces;
using SmartCafe.API.Domain.DTOs.Orders;

namespace SmartCafe.API.Controllers.Orders;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    // Khách hàng: Tạo đơn hàng mới (tự động tính giá từ DB, trừ tồn kho, trừ ví nội bộ & bắn SignalR cho bếp)
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto request)
    {
        try
        {
            var customerId = GetCurrentUserId();
            var result = await _orderService.CreateOrderAsync(customerId, request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (BadHttpRequestException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Lấy chi tiết 1 đơn hàng
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var order = await _orderService.GetByIdAsync(id);
        if (order == null)
        {
            return NotFound(new { message = $"Không tìm thấy đơn hàng với ID: {id}" });
        }

        // Bảo mật: Nếu không phải Admin/Staff thì chỉ được xem đơn của chính mình
        var isAdminOrStaff = User.IsInRole("Admin") || User.IsInRole("Staff");
        var currentUserId = GetCurrentUserId();
        if (!isAdminOrStaff && order.CustomerId != currentUserId)
        {
            return Forbid();
        }

        return Ok(order);
    }

    // Khách hàng: Lấy lịch sử đơn hàng cá nhân
    [HttpGet("me")]
    public async Task<IActionResult> GetMyOrders()
    {
        var customerId = GetCurrentUserId();
        var orders = await _orderService.GetMyOrdersAsync(customerId);
        return Ok(orders);
    }

    // Admin/Staff (Màn hình KDS): Lấy toàn bộ đơn hàng (có thể lọc theo ?status=Paid hoặc ?status=Preparing)
    [HttpGet]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> GetAllOrders([FromQuery] string? status = null)
    {
        var orders = await _orderService.GetAllOrdersAsync(status);
        return Ok(orders);
    }

    // Admin/Staff: Cập nhật tiến độ pha chế (Paid -> Preparing -> Ready -> Completed)
    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusRequestDto request)
    {
        try
        {
            var result = await _orderService.UpdateStatusAsync(id, request.Status);
            if (result == null)
            {
                return NotFound(new { message = $"Không tìm thấy đơn hàng với ID: {id}" });
            }

            return Ok(result);
        }
        catch (BadHttpRequestException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Khách hàng: Hủy đơn hàng (Chỉ thành công nếu trạng thái đang là Paid/PendingPayment -> Tự hoàn tiền 100% về ví)
    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> CancelMyOrder(Guid id)
    {
        try
        {
            var customerId = GetCurrentUserId();
            var result = await _orderService.CancelMyOrderAsync(customerId, id);
            if (result == null)
            {
                return NotFound(new { message = $"Không tìm thấy đơn hàng với ID: {id}" });
            }

            return Ok(result);
        }
        catch (BadHttpRequestException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? User.FindFirstValue("sub");

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Không xác định được danh tính người dùng từ Token.");
        }

        return userId;
    }
}