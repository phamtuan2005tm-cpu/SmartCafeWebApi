using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartCafe.API.Application.Interfaces;
using SmartCafe.API.Domain.DTOs.Gamification;
using SmartCafe.API.Domain.Entities;
using SmartCafe.API.Infrastructure.Data;

namespace SmartCafe.API.Application.Services;

public class VoucherService : IVoucherService
{
    private readonly SmartCafeDbContext _context;

    public VoucherService(SmartCafeDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<VoucherResponseDto>> GetAvailableVouchersForUserAsync(Guid userId)
    {
        var now = DateTime.UtcNow;

        // Lấy danh sách ID các Voucher công khai mà User này đã từng dùng
        var usedPublicVoucherIds = await _context.VoucherUsages
            .AsNoTracking()
            .Where(u => u.UserId == userId)
            .Select(u => u.VoucherId)
            .ToListAsync();

        var vouchers = await _context.Vouchers
            .AsNoTracking()
            .Where(v => v.IsActive
                        && v.ValidFrom <= now
                        && v.ValidTo >= now
                        && v.UsedCount < v.UsageLimit
                        && (v.UserId == userId || (v.UserId == null && !usedPublicVoucherIds.Contains(v.Id))))
            .OrderByDescending(v => v.UserId != null) // Ưu tiên hiển thị Voucher cá nhân lên đầu
            .ThenBy(v => v.ValidTo)
            .ToListAsync();

        return vouchers.Select(MapToDto);
    }

    public async Task<VoucherResponseDto> CreatePublicVoucherAsync(CreateVoucherRequestDto request)
    {
        var normalizedCode = request.Code.Trim().ToUpperInvariant();

        if (await _context.Vouchers.AnyAsync(v => v.Code == normalizedCode))
            throw new BadHttpRequestException($"Mã Voucher '{normalizedCode}' đã tồn tại.");

        if (request.DiscountType == "Percentage")
        {
            if (request.DiscountValue is <= 0 or > 100)
                throw new BadHttpRequestException("Phần trăm giảm giá phải nằm trong khoảng 1% - 100%.");
            if (!request.MaxDiscountAmount.HasValue || request.MaxDiscountAmount <= 0)
                throw new BadHttpRequestException("Voucher giảm theo % bắt buộc phải thiết lập Trần giảm tối đa (MaxDiscountAmount).");
        }

        var voucher = new Voucher
        {
            Id = Guid.NewGuid(),
            Code = normalizedCode,
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            UserId = null, // Công khai toàn quán
            DiscountType = request.DiscountType,
            DiscountValue = request.DiscountValue,
            MaxDiscountAmount = request.MaxDiscountAmount,
            MinOrderAmount = request.MinOrderAmount,
            UsageLimit = request.UsageLimit,
            UsedCount = 0,
            ValidFrom = request.ValidFrom.ToUniversalTime(),
            ValidTo = request.ValidTo.ToUniversalTime(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Vouchers.Add(voucher);
        await _context.SaveChangesAsync();

        return MapToDto(voucher);
    }

    public async Task<Voucher> GrantPersonalRewardVoucherAsync(
        Guid userId,
        string title,
        string description,
        string discountType,
        decimal discountValue,
        decimal? maxDiscount,
        decimal minOrderAmount,
        int validDays = 14)
    {
        var randomSuffix = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
        var voucher = new Voucher
        {
            Id = Guid.NewGuid(),
            Code = $"PET-{randomSuffix}",
            Title = title,
            Description = description,
            UserId = userId,
            DiscountType = discountType,
            DiscountValue = discountValue,
            MaxDiscountAmount = maxDiscount,
            MinOrderAmount = minOrderAmount,
            UsageLimit = 1,
            UsedCount = 0,
            ValidFrom = DateTime.UtcNow,
            ValidTo = DateTime.UtcNow.AddDays(validDays),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Vouchers.Add(voucher);
        // Lưu ý: Không gọi SaveChangesAsync ở đây nếu đang nằm chung transaction với PetService
        return voucher;
    }

    public async Task<(Voucher Voucher, decimal DiscountAmount)> ValidateAndCalculateDiscountAsync(Guid userId, string voucherCode, decimal subTotal)
    {
        var normalizedCode = voucherCode.Trim().ToUpperInvariant();
        var now = DateTime.UtcNow;

        var voucher = await _context.Vouchers
            .FirstOrDefaultAsync(v => v.Code == normalizedCode);

        if (voucher == null || !voucher.IsActive)
            throw new BadHttpRequestException($"Mã khuyến mãi '{normalizedCode}' không tồn tại hoặc đã bị khóa.");

        if (now < voucher.ValidFrom || now > voucher.ValidTo)
            throw new BadHttpRequestException($"Mã khuyến mãi '{normalizedCode}' chưa bắt đầu hoặc đã hết hạn.");

        if (voucher.UsedCount >= voucher.UsageLimit)
            throw new BadHttpRequestException($"Mã khuyến mãi '{normalizedCode}' đã hết lượt sử dụng.");

        if (voucher.UserId.HasValue && voucher.UserId.Value != userId)
            throw new BadHttpRequestException("Mã khuyến mãi này thuộc quyền sở hữu của tài khoản khác.");

        // Kiểm tra xem User đã dùng mã công khai này trước đó chưa (mỗi người tối đa 1 lần)
        if (!voucher.UserId.HasValue)
        {
            var alreadyUsed = await _context.VoucherUsages
                .AnyAsync(u => u.VoucherId == voucher.Id && u.UserId == userId);

            if (alreadyUsed)
                throw new BadHttpRequestException($"Bạn đã sử dụng mã khuyến mãi '{normalizedCode}' trước đó rồi.");
        }

        if (subTotal < voucher.MinOrderAmount)
            throw new BadHttpRequestException($"Đơn hàng cần đạt tối thiểu {voucher.MinOrderAmount:N0}đ để áp dụng mã '{normalizedCode}'.");

        // Tính toán số tiền được giảm
        decimal discountAmount;
        if (voucher.DiscountType == "Percentage")
        {
            discountAmount = Math.Round(subTotal * (voucher.DiscountValue / 100m), 0);
            if (voucher.MaxDiscountAmount.HasValue && discountAmount > voucher.MaxDiscountAmount.Value)
            {
                discountAmount = voucher.MaxDiscountAmount.Value;
            }
        }
        else
        {
            discountAmount = voucher.DiscountValue;
        }

        // Đảm bảo tiền giảm không vượt quá tổng tiền đơn
        if (discountAmount > subTotal)
            discountAmount = subTotal;

        return (voucher, discountAmount);
    }

    public Task RecordVoucherUsageAsync(Voucher voucher, Guid userId, Guid orderId, decimal discountApplied)
    {
        voucher.UsedCount += 1;

        var usage = new VoucherUsage
        {
            Id = Guid.NewGuid(),
            VoucherId = voucher.Id,
            UserId = userId,
            OrderId = orderId,
            DiscountApplied = discountApplied,
            UsedAt = DateTime.UtcNow
        };

        _context.VoucherUsages.Add(usage);
        return Task.CompletedTask;
    }

    public async Task RefundVoucherUsageAsync(Guid orderId)
    {
        var usage = await _context.VoucherUsages
            .Include(u => u.Voucher)
            .FirstOrDefaultAsync(u => u.OrderId == orderId);

        if (usage != null)
        {
            if (usage.Voucher.UsedCount > 0)
            {
                usage.Voucher.UsedCount -= 1;
            }
            _context.VoucherUsages.Remove(usage);
        }
    }

    private static VoucherResponseDto MapToDto(Voucher v) => new()
    {
        Id = v.Id,
        Code = v.Code,
        Title = v.Title,
        Description = v.Description,
        DiscountType = v.DiscountType,
        DiscountValue = v.DiscountValue,
        MaxDiscountAmount = v.MaxDiscountAmount,
        MinOrderAmount = v.MinOrderAmount,
        IsPersonal = v.UserId.HasValue,
        RemainingUses = Math.Max(0, v.UsageLimit - v.UsedCount),
        ValidFrom = v.ValidFrom,
        ValidTo = v.ValidTo
    };
}