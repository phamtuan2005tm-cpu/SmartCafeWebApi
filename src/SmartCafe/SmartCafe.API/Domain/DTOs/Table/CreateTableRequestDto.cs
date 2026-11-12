using System.ComponentModel.DataAnnotations;

namespace SmartCafe.API.Domain.DTOs.Tables;

public class CreateTableRequestDto
{
    [Required(ErrorMessage = "Số bàn không được để trống.")]
    [Range(1, 500, ErrorMessage = "Số bàn phải từ 1 đến 500.")]
    public int TableNumber { get; set; }

    [Required(ErrorMessage = "Tên bàn không được để trống.")]
    [MaxLength(50, ErrorMessage = "Tên bàn tối đa 50 ký tự.")]
    public string TableName { get; set; } = string.Empty;

    [Range(1, 50, ErrorMessage = "Sức chứa phải từ 1 đến 50 người.")]
    public int Capacity { get; set; } = 4;
}