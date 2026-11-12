using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCafe.API.Application.Interfaces;
using SmartCafe.API.Domain.DTOs.Tables;

namespace SmartCafe.API.Controllers.Tables;

[ApiController]
[Route("api/[controller]")]
public class TablesController : ControllerBase
{
    private readonly ITableService _tableService;

    public TablesController(ITableService tableService)
    {
        _tableService = tableService;
    }

    // Public/Staff: Lấy danh sách toàn bộ bàn
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tables = await _tableService.GetAllAsync();
        return Ok(tables);
    }

    // Public: Lấy thông tin bàn theo ID
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var table = await _tableService.GetByIdAsync(id);
        if (table == null)
        {
            return NotFound(new { message = $"Không tìm thấy bàn với ID: {id}" });
        }

        return Ok(table);
    }

    // Public: Khách quét mã QR tại bàn -> Frontend gọi API này để xác nhận đúng bàn & lấy TableId gắn vào giỏ hàng
    [HttpGet("scan/{qrCodeToken}")]
    public async Task<IActionResult> ScanQrCode(string qrCodeToken)
    {
        try
        {
            var table = await _tableService.GetByQrTokenAsync(qrCodeToken);
            if (table == null)
            {
                return NotFound(new { message = "Mã QR bàn không hợp lệ hoặc bàn đã ngừng hoạt động." });
            }

            return Ok(table);
        }
        catch (BadHttpRequestException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Admin Only: Tạo bàn mới (tự sinh QrCodeToken)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateTableRequestDto request)
    {
        try
        {
            var result = await _tableService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (BadHttpRequestException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Admin Only: Cập nhật thông tin bàn
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTableRequestDto request)
    {
        try
        {
            var result = await _tableService.UpdateAsync(id, request);
            if (result == null)
            {
                return NotFound(new { message = $"Không tìm thấy bàn với ID: {id}" });
            }

            return Ok(result);
        }
        catch (BadHttpRequestException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Admin/Staff: Đổi nhanh trạng thái bàn (Trống / Có khách / Bảo trì)
    [HttpPatch("{id:guid}/status")]
    [Authorize]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateTableStatusRequestDto request)
    {
        var result = await _tableService.UpdateStatusAsync(id, request.Status);
        if (result == null)
        {
            return NotFound(new { message = $"Không tìm thấy bàn với ID: {id}" });
        }

        return Ok(result);
    }

    // Admin Only: Xóa bàn
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var isSuccess = await _tableService.DeleteAsync(id);
        if (!isSuccess)
        {
            return NotFound(new { message = $"Không tìm thấy bàn với ID: {id}" });
        }

        return Ok(new { message = "Xóa bàn thành công." });
    }
}