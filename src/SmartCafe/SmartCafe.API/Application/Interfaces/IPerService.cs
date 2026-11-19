using SmartCafe.API.Domain.DTOs.Gamification;

namespace SmartCafe.API.Application.Interfaces;

public interface IPetService
{
    Task<PetResponseDto> GetOrCreateMyPetAsync(Guid userId);
    Task<PetResponseDto> CustomizePetAsync(Guid userId, CustomizePetRequestDto request);
    Task AddExpFromCompletedOrderAsync(Guid userId, decimal orderTotalAmount);
}