using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCafe.API.Application.Interfaces;
using SmartCafe.API.Domain.DTOs.SavedDrinks;

namespace SmartCafe.API.Controllers.SavedDrinks;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SavedCustomDrinksController : ControllerBase
{
    private readonly ISavedCustomDrinkService _savedDrinkService;

    public SavedCustomDrinksController(ISavedCustomDrinkService savedDrinkService)
    {
        _savedDrinkService = savedDrinkService;
    }

    // Lấy danh sách công thức đồ uống cá nhân đã lưu
    [HttpGet("me")]
    public async Task<IActionResult> GetMyDrinks()
    {
        var userId = GetCurrentUserId();
        var drinks = await _savedDrinkService.GetMyDrinksAsync(userId);
        return Ok(drinks);
    }

    // Lưu một công thức đồ uống tùy chỉnh mới
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSavedDrinkRequestDto request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var created = await _savedDrinkService.CreateAsync(userId, request);
            return CreatedAtAction(nameof(GetMyDrinks), new { }, created);
        }
        catch (BadHttpRequestException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Đặt hàng 1 chạm trực tiếp từ công thức đã lưu
    [HttpPost("{id:guid}/order")]
    public async Task<IActionResult> OrderFromSaved(Guid id, [FromBody] OrderSavedDrinkRequestDto request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var orderResult = await _savedDrinkService.OrderFromSavedAsync(userId, id, request);
            return Ok(orderResult);
        }
        catch (BadHttpRequestException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Xóa công thức đã lưu
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetCurrentUserId();
        var deleted = await _savedDrinkService.DeleteAsync(userId, id);
        if (!deleted)
        {
            return NotFound(new { message = "Không tìm thấy công thức đồ uống cần xóa." });
        }

        return NoContent();
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