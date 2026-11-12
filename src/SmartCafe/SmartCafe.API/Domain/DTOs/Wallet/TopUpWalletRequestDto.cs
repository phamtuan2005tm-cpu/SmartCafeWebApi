using System.ComponentModel.DataAnnotations;

namespace SmartCafe.API.Domain.DTOs.Wallets;

public class TopUpWalletRequestDto
{
    [Required(ErrorMessage = "Số tiền nạp không được để trống.")]
    [Range(10000, 10000000, ErrorMessage = "Số tiền nạp mỗi lần phải từ 10.000đ đến 10.000.000đ.")]
    public decimal Amount { get; set; }

    [MaxLength(200)]
    public string? Note { get; set; }
}