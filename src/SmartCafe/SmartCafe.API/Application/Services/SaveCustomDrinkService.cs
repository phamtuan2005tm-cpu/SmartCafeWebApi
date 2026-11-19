using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartCafe.API.Application.Interfaces;
using SmartCafe.API.Domain.DTOs.Orders;
using SmartCafe.API.Domain.DTOs.SavedDrinks;
using SmartCafe.API.Domain.Entities;
using SmartCafe.API.Infrastructure.Data;

namespace SmartCafe.API.Application.Services;

public class SavedCustomDrinkService : ISavedCustomDrinkService
{
    private readonly SmartCafeDbContext _context;
    private readonly IOrderService _orderService;

    public SavedCustomDrinkService(SmartCafeDbContext context, IOrderService orderService)
    {
        _context = context;
        _orderService = orderService;
    }

    public async Task<IEnumerable<SavedDrinkResponseDto>> GetMyDrinksAsync(Guid userId)
    {
        var savedDrinks = await _context.SavedCustomDrinks
            .AsNoTracking()
            .Include(s => s.Product)
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        if (!savedDrinks.Any())
            return Enumerable.Empty<SavedDrinkResponseDto>();

        // Giải mã toàn bộ OptionIds của các công thức và nạp 1 lần từ DB (tránh N+1 query)
        var parsedMap = savedDrinks.ToDictionary(
            s => s.Id,
            s => ParseOptionIds(s.SelectedOptionIdsJson));

        var allOptionIds = parsedMap.Values
            .SelectMany(ids => ids)
            .Distinct()
            .ToList();

        var optionsDict = await _context.Options
            .AsNoTracking()
            .Where(o => allOptionIds.Contains(o.Id))
            .ToDictionaryAsync(o => o.Id);

        return savedDrinks.Select(s =>
        {
            var optionIds = parsedMap[s.Id];
            var matchedOptions = optionIds
                .Where(id => optionsDict.ContainsKey(id))
                .Select(id => optionsDict[id])
                .ToList();

            var optionsTotal = matchedOptions.Sum(o => o.AdditionalPrice);

            return new SavedDrinkResponseDto
            {
                Id = s.Id,
                CustomName = s.CustomName,
                ProductId = s.ProductId,
                ProductName = s.Product?.Name ?? "Sản phẩm",
                ProductImageUrl = s.Product?.ImageUrl,
                BasePrice = s.Product?.Price ?? 0,
                EstimatedUnitPrice = (s.Product?.Price ?? 0) + optionsTotal,
                CreatedAt = s.CreatedAt,
                Options = matchedOptions.Select(o => new SavedDrinkOptionDto
                {
                    OptionId = o.Id,
                    OptionName = o.Name,
                    AdditionalPrice = o.AdditionalPrice
                }).ToList()
            };
        });
    }

    public async Task<SavedDrinkResponseDto> CreateAsync(Guid userId, CreateSavedDrinkRequestDto request)
    {
        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.ProductId);

        if (product == null)
            throw new BadHttpRequestException($"Không tìm thấy sản phẩm với ID: {request.ProductId}");

        var distinctOptionIds = request.SelectedOptionIds.Distinct().ToList();
        var validOptions = await _context.Options
            .AsNoTracking()
            .Where(o => distinctOptionIds.Contains(o.Id))
            .ToListAsync();

        if (validOptions.Count != distinctOptionIds.Count)
            throw new BadHttpRequestException("Một hoặc nhiều tùy chọn (OptionId) không hợp lệ.");

        var entity = new SavedCustomDrink
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ProductId = product.Id,
            CustomName = request.CustomName.Trim(),
            SelectedOptionIdsJson = JsonSerializer.Serialize(distinctOptionIds),
            CreatedAt = DateTime.UtcNow
        };

        _context.SavedCustomDrinks.Add(entity);
        await _context.SaveChangesAsync();

        var optionsTotal = validOptions.Sum(o => o.AdditionalPrice);

        return new SavedDrinkResponseDto
        {
            Id = entity.Id,
            CustomName = entity.CustomName,
            ProductId = product.Id,
            ProductName = product.Name,
            ProductImageUrl = product.ImageUrl,
            BasePrice = product.Price,
            EstimatedUnitPrice = product.Price + optionsTotal,
            CreatedAt = entity.CreatedAt,
            Options = validOptions.Select(o => new SavedDrinkOptionDto
            {
                OptionId = o.Id,
                OptionName = o.Name,
                AdditionalPrice = o.AdditionalPrice
            }).ToList()
        };
    }

    public async Task<OrderResponseDto> OrderFromSavedAsync(Guid userId, Guid savedDrinkId, OrderSavedDrinkRequestDto request)
    {
        var savedDrink = await _context.SavedCustomDrinks
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == savedDrinkId && s.UserId == userId);

        if (savedDrink == null)
            throw new BadHttpRequestException("Không tìm thấy công thức đồ uống đã lưu của bạn.");

        var optionIds = ParseOptionIds(savedDrink.SelectedOptionIdsJson);

        // Chuyển đổi trực tiếp thành CreateOrderRequestDto để dùng lại toàn bộ lõi giao dịch của OrderService
        var orderRequest = new CreateOrderRequestDto
        {
            OrderType = request.OrderType,
            TableId = request.TableId,
            ScheduledPickupTime = request.ScheduledPickupTime,
            Note = string.IsNullOrWhiteSpace(request.Note)
                ? $"Đặt từ công thức: {savedDrink.CustomName}"
                : request.Note,
            Items = new List<CreateOrderItemDto>
            {
                new()
                {
                    ProductId = savedDrink.ProductId,
                    Quantity = request.Quantity,
                    SelectedOptionIds = optionIds
                }
            }
        };

        return await _orderService.CreateOrderAsync(userId, orderRequest);
    }

    public async Task<bool> DeleteAsync(Guid userId, Guid savedDrinkId)
    {
        var savedDrink = await _context.SavedCustomDrinks
            .FirstOrDefaultAsync(s => s.Id == savedDrinkId && s.UserId == userId);

        if (savedDrink == null)
            return false;

        _context.SavedCustomDrinks.Remove(savedDrink);
        await _context.SaveChangesAsync();
        return true;
    }

    private static List<Guid> ParseOptionIds(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<List<Guid>>(json) ?? new List<Guid>();
        }
        catch
        {
            return new List<Guid>();
        }
    }
}