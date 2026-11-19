using SmartCafe.API.Domain.DTOs.Orders;
using SmartCafe.API.Domain.DTOs.SavedDrinks;

namespace SmartCafe.API.Application.Interfaces;

public interface ISavedCustomDrinkService
{
    Task<IEnumerable<SavedDrinkResponseDto>> GetMyDrinksAsync(Guid userId);
    Task<SavedDrinkResponseDto> CreateAsync(Guid userId, CreateSavedDrinkRequestDto request);
    Task<OrderResponseDto> OrderFromSavedAsync(Guid userId, Guid savedDrinkId, OrderSavedDrinkRequestDto request);
    Task<bool> DeleteAsync(Guid userId, Guid savedDrinkId);
}