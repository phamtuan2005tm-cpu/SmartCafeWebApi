using System.ComponentModel.DataAnnotations;

namespace SmartCafe.API.Domain.DTOs.Products;

public class UpdateProductRequestDto
{
    [Required(ErrorMessage = "Mã danh mục không được để trống.")]
    public Guid CategoryId { get; set; }

    [Required(ErrorMessage = "Tên sản phẩm không được để trống.")]
    [StringLength(100, ErrorMessage = "Tên sản phẩm không được vượt quá 100 ký tự.")]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn hoặc bằng 0.")]
    public decimal Price { get; set; }

    public string? ImageUrl { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho phải lớn hơn hoặc bằng 0.")]
    public int StockQuantity { get; set; }

    public bool IsAvailable { get; set; }
}