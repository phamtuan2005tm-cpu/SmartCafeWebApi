using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCafe.API.Application.Interfaces;
using SmartCafe.API.Domain.DTOs.Categories;

namespace SmartCafe.API.Controllers.Categories;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    // Public / Unauthenticated
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _categoryService.GetAllAsync();
        return Ok(categories);
    }

    // Public / Unauthenticated
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var category = await _categoryService.GetByIdAsync(id);

        if (category == null)
        {
            return NotFound(new { message = $"Không tìm thấy danh mục với ID: {id}" });
        }

        return Ok(category);
    }

    // Admin Only
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequestDto request)
    {
        var result = await _categoryService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    // Admin Only
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequestDto request)
    {
        var result = await _categoryService.UpdateAsync(id, request);

        if (result == null)
        {
            return NotFound(new { message = $"Không tìm thấy danh mục với ID: {id}" });
        }

        return Ok(result);
    }

    // Admin Only
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var isSuccess = await _categoryService.DeleteAsync(id);

        if (!isSuccess)
        {
            return NotFound(new { message = $"Không tìm thấy danh mục với ID: {id}" });
        }

        return Ok(new { message = "Xóa danh mục thành công." });
    }
}