using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCafe.API.Application.Interfaces;
using SmartCafe.API.Domain.DTOs.Products; // Hoặc .Product tùy namespace DTO của bạn

namespace SmartCafe.API.Controllers.Products;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // Public / Unauthenticated
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }

    // Public / Unauthenticated
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);

        if (product == null)
        {
            return NotFound(new { message = $"Không tìm thấy sản phẩm với ID: {id}" });
        }

        return Ok(product);
    }

    // Admin Only
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateProductRequestDto request)
    {
        try
        {
            var result = await _productService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (BadHttpRequestException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Admin Only
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequestDto request)
    {
        try
        {
            var result = await _productService.UpdateAsync(id, request);

            if (result == null)
            {
                return NotFound(new { message = $"Không tìm thấy sản phẩm với ID: {id}" });
            }

            return Ok(result);
        }
        catch (BadHttpRequestException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Admin Only
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var isSuccess = await _productService.DeleteAsync(id);

        if (!isSuccess)
        {
            return NotFound(new { message = $"Không tìm thấy sản phẩm với ID: {id}" });
        }

        return Ok(new { message = "Xóa sản phẩm thành công." });
    }
}