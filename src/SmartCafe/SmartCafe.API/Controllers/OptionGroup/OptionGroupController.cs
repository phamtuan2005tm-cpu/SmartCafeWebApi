using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCafe.API.Application.Interfaces;
using SmartCafe.API.Domain.DTOs.Options;

namespace SmartCafe.API.Controllers.Options;

[ApiController]
[Route("api/[controller]")]
public class OptionGroupsController : ControllerBase
{
    private readonly IOptionGroupService _optionGroupService;

    public OptionGroupsController(IOptionGroupService optionGroupService)
    {
        _optionGroupService = optionGroupService;
    }

    // Public: Lấy toàn bộ nhóm tùy chọn kèm các option con
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var groups = await _optionGroupService.GetAllAsync();
        return Ok(groups);
    }

    // Public: Lấy chi tiết 1 nhóm tùy chọn theo ID
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var group = await _optionGroupService.GetByIdAsync(id);
        if (group == null)
        {
            return NotFound(new { message = $"Không tìm thấy nhóm tùy chọn với ID: {id}" });
        }

        return Ok(group);
    }

    // Public: Lấy danh sách nhóm tùy chọn đang gắn với 1 sản phẩm (Dùng cho UI Custom món)
    [HttpGet("product/{productId:guid}")]
    public async Task<IActionResult> GetByProductId(Guid productId)
    {
        try
        {
            var groups = await _optionGroupService.GetByProductIdAsync(productId);
            return Ok(groups);
        }
        catch (BadHttpRequestException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Admin Only: Tạo nhóm tùy chọn mới (VD: Chọn Size, Mức Đường, Topping)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateOptionGroupRequestDto request)
    {
        try
        {
            var result = await _optionGroupService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (BadHttpRequestException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Admin Only: Cập nhật thông tin nhóm tùy chọn
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOptionGroupRequestDto request)
    {
        try
        {
            var result = await _optionGroupService.UpdateAsync(id, request);
            if (result == null)
            {
                return NotFound(new { message = $"Không tìm thấy nhóm tùy chọn với ID: {id}" });
            }

            return Ok(result);
        }
        catch (BadHttpRequestException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Admin Only: Xóa nhóm tùy chọn
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var isSuccess = await _optionGroupService.DeleteAsync(id);
        if (!isSuccess)
        {
            return NotFound(new { message = $"Không tìm thấy nhóm tùy chọn với ID: {id}" });
        }

        return Ok(new { message = "Xóa nhóm tùy chọn thành công." });
    }

    // Admin Only: Gán danh sách nhóm tùy chọn vào một Sản phẩm cụ thể
    [HttpPost("product/{productId:guid}/assign")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignToProduct(Guid productId, [FromBody] AssignOptionGroupsRequestDto request)
    {
        try
        {
            await _optionGroupService.AssignToProductAsync(productId, request);
            return Ok(new { message = "Cập nhật danh sách tùy chọn cho sản phẩm thành công." });
        }
        catch (BadHttpRequestException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}