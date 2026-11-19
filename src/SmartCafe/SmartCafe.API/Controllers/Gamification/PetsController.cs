using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCafe.API.Application.Interfaces;
using SmartCafe.API.Domain.DTOs.Gamification;

namespace SmartCafe.API.Controllers.Gamification;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PetsController : ControllerBase
{
    private readonly IPetService _petService;

    public PetsController(IPetService petService)
    {
        _petService = petService;
    }

    // Lấy thông tin thú cưng của tôi (tự động nhận nuôi mặc định nếu chưa có)
    [HttpGet("me")]
    public async Task<IActionResult> GetMyPet()
    {
        var userId = GetCurrentUserId();
        var pet = await _petService.GetOrCreateMyPetAsync(userId);
        return Ok(pet);
    }

    // Đổi tên hoặc chọn loài thú cưng (Capybara, Cat, Dog, Bunny)
    [HttpPut("me")]
    public async Task<IActionResult> CustomizeMyPet([FromBody] CustomizePetRequestDto request)
    {
        var userId = GetCurrentUserId();
        var updatedPet = await _petService.CustomizePetAsync(userId, request);
        return Ok(updatedPet);
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