using Microsoft.AspNetCore.Mvc;
using SmartCafe.API.Application.Interfaces;
using SmartCafe.API.Domain.DTOs.Auth;

namespace SmartCafe.API.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// API Đăng nhập tài khoản
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request);
        if (result == null)
            return Unauthorized(new { message = "Email hoặc mật khẩu không chính xác, hoặc tài khoản đã bị khóa!" });

        return Ok(result);
    }

    /// <summary>
    /// API Đăng ký tài khoản mới (Mặc định Role Customer)
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        var result = await _authService.RegisterAsync(request);
        if (result == null)
            return BadRequest(new { message = "Email này đã tồn tại trong hệ thống!" });

        return Ok(result);
    }

    /// <summary>
    /// API Cấp mới Access Token bằng Refresh Token khi Access Token bị hết hạn
    /// </summary>
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        var result = await _authService.RefreshTokenAsync(request);
        if (result == null)
            return Unauthorized(new { message = "Refresh Token không hợp lệ, đã bị thu hồi hoặc hết hạn!" });

        return Ok(result);
    }

    /// <summary>
    /// API Đăng xuất / Thu hồi Refresh Token
    /// </summary>
    [HttpPost("revoke-token")]
    public async Task<IActionResult> RevokeToken([FromBody] string refreshToken)
    {
        var result = await _authService.RevokeTokenAsync(refreshToken);
        if (!result)
            return BadRequest(new { message = "Refresh Token không tồn tại hoặc đã bị thu hồi trước đó!" });

        return Ok(new { message = "Thu hồi Refresh Token thành công!" });
    }
}