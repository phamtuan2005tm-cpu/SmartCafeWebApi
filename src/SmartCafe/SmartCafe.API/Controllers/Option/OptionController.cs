using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCafe.API.Application.Interfaces;
using SmartCafe.API.Domain.DTOs.Options;

namespace SmartCafe.API.Controllers.Options;

[ApiController]
[Route("api/[controller]")]
public class OptionsController : ControllerBase
{
    private readonly IOptionService _optionService;

    public OptionsController(IOptionService optionService)
    {
        _optionService = optionService;
    }

    // Public: Lấy tất cả tùy chọn con thuộc một OptionGroup
    [HttpGet("group/{optionGroupId:guid}")]
    public async Task<IActionResult> GetByGroupId(Guid optionGroupId)
    {
        var options = await _optionService.GetByGroupIdAsync(optionGroupId);
        return Ok(options);
    }

    // Public: Lấy chi tiết 1 tùy chọn theo ID
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var option = await _optionService.GetByIdAsync(id);
        if (option == null)
        {
            return NotFound(new { message = $"Không tìm thấy tùy chọn với ID: {id}" });
        }

        return Ok(option);
    }

    // Admin Only: Thêm tùy chọn mới (VD: Size L +6000đ, Trân châu trắng +5000đ)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateOptionRequestDto request)
    {
        try
        {
            var result = await _optionService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (BadHttpRequestException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Admin Only: Cập nhật tùy chọn
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOptionRequestDto request)
    {
        try
        {
            var result = await _optionService.UpdateAsync(id, request);
            if (result == null)
            {
                return NotFound(new { message = $"Không tìm thấy tùy chọn với ID: {id}" });
            }

            return Ok(result);
        }
        catch (BadHttpRequestException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Admin Only: Xóa tùy chọn
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var isSuccess = await _optionService.DeleteAsync(id);
        if (!isSuccess)
        {
            return NotFound(new { message = $"Không tìm thấy tùy chọn với ID: {id}" });
        }

        return Ok(new { message = "Xóa tùy chọn thành công." });
    }
}