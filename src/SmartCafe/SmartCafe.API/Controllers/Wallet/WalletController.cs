using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCafe.API.Application.Interfaces;
using SmartCafe.API.Domain.DTOs.Wallets;

namespace SmartCafe.API.Controllers.Wallets;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Bắt buộc đăng nhập mới được thao tác với Ví
public class WalletsController : ControllerBase
{
    private readonly IWalletService _walletService;

    public WalletsController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    // Lấy thông tin ví & số dư hiện tại của người dùng đang đăng nhập (tự khởi tạo nếu chưa có)
    [HttpGet("me")]
    public async Task<IActionResult> GetMyWallet()
    {
        var userId = GetCurrentUserId();
        var wallet = await _walletService.GetOrCreateWalletAsync(userId);
        return Ok(wallet);
    }

    // Lấy lịch sử biến động số dư (Nạp tiền, Thanh toán đơn, Hoàn tiền)
    [HttpGet("me/transactions")]
    public async Task<IActionResult> GetMyTransactions()
    {
        var userId = GetCurrentUserId();
        var transactions = await _walletService.GetTransactionsAsync(userId);
        return Ok(transactions);
    }

    // Nạp tiền vào ví cá nhân
    [HttpPost("top-up")]
    public async Task<IActionResult> TopUp([FromBody] TopUpWalletRequestDto request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var wallet = await _walletService.TopUpAsync(userId, request);
            return Ok(wallet);
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