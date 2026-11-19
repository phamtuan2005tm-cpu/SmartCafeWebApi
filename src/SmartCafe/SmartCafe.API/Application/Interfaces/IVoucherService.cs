using SmartCafe.API.Domain.DTOs.Gamification;
using SmartCafe.API.Domain.Entities;

namespace SmartCafe.API.Application.Interfaces;

public interface IVoucherService
{
    Task<IEnumerable<VoucherResponseDto>> GetAvailableVouchersForUserAsync(Guid userId);
    Task<VoucherResponseDto> CreatePublicVoucherAsync(CreateVoucherRequestDto request);
    Task<Voucher> GrantPersonalRewardVoucherAsync(Guid userId, string title, string description, string discountType, decimal discountValue, decimal? maxDiscount, decimal minOrderAmount, int validDays = 14);
    Task<(Voucher Voucher, decimal DiscountAmount)> ValidateAndCalculateDiscountAsync(Guid userId, string voucherCode, decimal subTotal);
    Task RecordVoucherUsageAsync(Voucher voucher, Guid userId, Guid orderId, decimal discountApplied);
    Task RefundVoucherUsageAsync(Guid orderId);
}