using Microsoft.EntityFrameworkCore;
using SmartCafe.API.Application.Interfaces;
using SmartCafe.API.Domain.DTOs.Gamification;
using SmartCafe.API.Domain.Entities;
using SmartCafe.API.Infrastructure.Data;

namespace SmartCafe.API.Application.Services;

public class PetService : IPetService
{
    private readonly SmartCafeDbContext _context;
    private readonly IVoucherService _voucherService;

    public PetService(SmartCafeDbContext context, IVoucherService voucherService)
    {
        _context = context;
        _voucherService = voucherService;
    }

    public async Task<PetResponseDto> GetOrCreateMyPetAsync(Guid userId)
    {
        var pet = await _context.UserPets
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (pet == null)
        {
            pet = new UserPet
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = "Capy Barista",
                PetType = "Capybara",
                Level = 1,
                CurrentExp = 0,
                AdoptedAt = DateTime.UtcNow
            };

            _context.UserPets.Add(pet);
            await _context.SaveChangesAsync();
        }

        return MapToDto(pet);
    }

    public async Task<PetResponseDto> CustomizePetAsync(Guid userId, CustomizePetRequestDto request)
    {
        var pet = await _context.UserPets
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (pet == null)
        {
            pet = new UserPet
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                AdoptedAt = DateTime.UtcNow
            };
            _context.UserPets.Add(pet);
        }

        pet.Name = request.Name.Trim();
        pet.PetType = request.PetType;

        await _context.SaveChangesAsync();
        return MapToDto(pet);
    }

    public async Task AddExpFromCompletedOrderAsync(Guid userId, decimal orderTotalAmount)
    {
        if (orderTotalAmount <= 0) return;

        var pet = await _context.UserPets
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (pet == null)
        {
            pet = new UserPet
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = "Capy Barista",
                PetType = "Capybara",
                Level = 1,
                CurrentExp = 0,
                AdoptedAt = DateTime.UtcNow
            };
            _context.UserPets.Add(pet);
        }

        // Quy đổi: 1.000 VNĐ = 1 EXP (VD: ly 45.000đ = +45 EXP)
        int earnedExp = (int)Math.Floor(orderTotalAmount / 1000m);
        if (earnedExp <= 0) return;

        pet.CurrentExp += earnedExp;
        pet.LastFedAt = DateTime.UtcNow;

        // Vòng lặp kiểm tra lên cấp (có thể lên 1 hoặc nhiều cấp nếu đơn lớn)
        while (pet.CurrentExp >= CalculateExpForNextLevel(pet.Level))
        {
            pet.CurrentExp -= CalculateExpForNextLevel(pet.Level);
            pet.Level += 1;

            // Tự động tặng Voucher thưởng mỗi khi Pet đạt cấp độ mới
            await GrantLevelUpRewardAsync(userId, pet);
        }
    }

    private async Task GrantLevelUpRewardAsync(Guid userId, UserPet pet)
    {
        // Cấp chia hết cho 5 (Lv 5, 10, 15...): Thưởng lớn giảm 30% tối đa 30.000đ
        if (pet.Level % 5 == 0)
        {
            await _voucherService.GrantPersonalRewardVoucherAsync(
                userId: userId,
                title: $"Quà Mốc VIP: {pet.Name} đạt Level {pet.Level}!",
                description: "Giảm 30% (tối đa 30.000đ) cho đơn hàng bất kỳ.",
                discountType: "Percentage",
                discountValue: 30,
                maxDiscount: 30000,
                minOrderAmount: 40000,
                validDays: 30);
        }
        else
        {
            // Các cấp độ thường: Thưởng giảm thẳng 10.000đ cho đơn từ 30.000đ
            await _voucherService.GrantPersonalRewardVoucherAsync(
                userId: userId,
                title: $"Mừng {pet.Name} thăng cấp Level {pet.Level}!",
                description: "Giảm ngay 10.000đ cho đơn từ 30.000đ.",
                discountType: "FixedAmount",
                discountValue: 10000,
                maxDiscount: null,
                minOrderAmount: 30000,
                validDays: 14);
        }
    }

    private static int CalculateExpForNextLevel(int currentLevel) => currentLevel * 100;

    private static PetResponseDto MapToDto(UserPet pet) => new()
    {
        Id = pet.Id,
        Name = pet.Name,
        PetType = pet.PetType,
        Level = pet.Level,
        CurrentExp = pet.CurrentExp,
        ExpToNextLevel = CalculateExpForNextLevel(pet.Level),
        AgeInDays = Math.Max(1, (int)(DateTime.UtcNow - pet.AdoptedAt).TotalDays + 1),
        AdoptedAt = pet.AdoptedAt,
        LastFedAt = pet.LastFedAt
    };
}