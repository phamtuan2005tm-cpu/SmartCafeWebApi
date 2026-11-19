using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCafe.API.Application.Interfaces;
using SmartCafe.API.Domain.DTOs.Gamification;

namespace SmartCafe.API.Controllers.Gamification;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VouchersController : ControllerBase
{
    private readonly IVoucherService _voucherService;

    public VouchersController(IVoucherService voucherService)
    {
        _voucherService = voucherService;
    }

    // Khách hàng xem danh sách Voucher khả dụng (gồm Voucher cá nhân từ Pet + Voucher toàn quán)
    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableVouchers()
    {
        var userId = GetCurrentUserId();
        var vouchers = await _voucherService.GetAvailableVouchersForUserAsync(userId);
        return Ok(vouchers);
    }

    // Admin tạo chiến dịch Voucher công khai toàn quán
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreatePublicVoucher([FromBody] CreateVoucherRequestDto request)
    {
        try
        {
            var created = await _voucherService.CreatePublicVoucherAsync(request);
            return CreatedAtAction(nameof(GetAvailableVouchers), new { }, created);
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
            throw new UnauthorizedAccessException("Không xác định được danh tính người dùng từ Token.");

        return userId;
    }
}