using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SmartCafe.API.Application.Interfaces;
using SmartCafe.API.Domain.DTOs.Orders;
using SmartCafe.API.Domain.Entities;
using SmartCafe.API.Hubs;
using SmartCafe.API.Infrastructure.Data;

namespace SmartCafe.API.Application.Services;

public class OrderService : IOrderService
{
    private readonly SmartCafeDbContext _context;
    private readonly IWalletService _walletService;
    private readonly IHubContext<OrderHub> _hubContext;

    public OrderService(
        SmartCafeDbContext context,
        IWalletService walletService,
        IHubContext<OrderHub> hubContext)
    {
        _context = context;
        _walletService = walletService;
        _hubContext = hubContext;
    }

    public async Task<OrderResponseDto> CreateOrderAsync(Guid customerId, CreateOrderRequestDto request)
    {
        // 1. Kiểm tra thông tin bàn nếu đặt tại quán (DineIn)
        Table? table = null;
        if (request.OrderType == "DineIn")
        {
            if (!request.TableId.HasValue)
                throw new BadHttpRequestException("Đơn uống tại quán (DineIn) bắt buộc phải chọn bàn.");

            table = await _context.Tables.FirstOrDefaultAsync(t => t.Id == request.TableId.Value && t.IsActive);
            if (table == null)
                throw new BadHttpRequestException("Bàn không tồn tại hoặc đã ngừng phục vụ.");

            if (table.Status == "Maintenance")
                throw new BadHttpRequestException($"Bàn {table.TableNumber} hiện đang bảo trì.");
        }

        // 2. Nạp trước toàn bộ Product và Option liên quan từ DB để tính giá chính xác
        var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        var allOptionIds = request.Items
            .SelectMany(i => i.SelectedOptionIds)
            .Distinct()
            .ToList();

        var options = await _context.Options
            .Where(o => allOptionIds.Contains(o.Id))
            .ToDictionaryAsync(o => o.Id);

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                TableId = request.OrderType == "DineIn" ? request.TableId : null,
                OrderType = request.OrderType,
                Status = "PendingPayment",
                ScheduledPickupTime = request.OrderType == "Pickup" ? request.ScheduledPickupTime : null,
                Note = request.Note,
                CreatedAt = DateTime.UtcNow
            };

            decimal grandTotal = 0;

            foreach (var itemDto in request.Items)
            {
                if (!products.TryGetValue(itemDto.ProductId, out var product))
                    throw new BadHttpRequestException($"Không tìm thấy sản phẩm với ID: {itemDto.ProductId}");

                if (!product.IsAvailable)
                    throw new BadHttpRequestException($"Món '{product.Name}' hiện đang tạm ngưng phục vụ.");

                if (product.StockQuantity < itemDto.Quantity)
                    throw new BadHttpRequestException($"Món '{product.Name}' không đủ số lượng (Còn lại: {product.StockQuantity}).");

                // Trừ tồn kho
                product.StockQuantity -= itemDto.Quantity;

                var orderDetail = new OrderDetail
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = product.Id,
                    Quantity = itemDto.Quantity,
                    UnitPrice = product.Price
                };

                decimal optionsPricePerCup = 0;

                foreach (var optId in itemDto.SelectedOptionIds.Distinct())
                {
                    if (!options.TryGetValue(optId, out var option))
                        throw new BadHttpRequestException($"Tùy chọn (Option) không hợp lệ: {optId}");

                    optionsPricePerCup += option.AdditionalPrice;

                    orderDetail.OrderDetailOptions.Add(new OrderDetailOption
                    {
                        Id = Guid.NewGuid(),
                        OrderDetailId = orderDetail.Id,
                        OptionId = option.Id,
                        OptionNameSnapshot = option.Name,
                        AdditionalPriceSnapshot = option.AdditionalPrice
                    });
                }

                decimal itemTotal = (product.Price + optionsPricePerCup) * itemDto.Quantity;
                grandTotal += itemTotal;

                order.OrderDetails.Add(orderDetail);
            }

            order.TotalAmount = grandTotal;
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // 3. Thanh toán trước qua Ví nội bộ SmartCafe
            var shortCode = order.Id.ToString("N")[..6].ToUpper();
            await _walletService.DeductForOrderAsync(
                customerId,
                order.Id,
                grandTotal,
                $"Thanh toán đơn hàng #{shortCode}");

            // Cập nhật trạng thái đơn sang Paid & cập nhật trạng thái bàn sang Occupied
            order.Status = "Paid";
            order.UpdatedAt = DateTime.UtcNow;

            if (table != null && table.Status == "Available")
            {
                table.Status = "Occupied";
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            var responseDto = await GetByIdAsync(order.Id)
                ?? throw new Exception("Lỗi khi tải lại thông tin đơn hàng vừa tạo.");

            // 4. Bắn SignalR đẩy đơn mới xuống màn hình pha chế (KDS)
            await _hubContext.Clients.Group("KitchenStaff")
                .SendAsync("ReceiveNewOrder", responseDto);

            return responseDto;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<OrderResponseDto?> GetByIdAsync(Guid id)
    {
        var order = await _context.Orders
            .AsNoTracking()
            .Include(o => o.Customer)
            .Include(o => o.Table)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.OrderDetailOptions)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return null;

        return MapToDto(order);
    }

    public async Task<IEnumerable<OrderResponseDto>> GetMyOrdersAsync(Guid customerId)
    {
        var orders = await _context.Orders
            .AsNoTracking()
            .Include(o => o.Customer)
            .Include(o => o.Table)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.OrderDetailOptions)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return orders.Select(MapToDto);
    }

    public async Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync(string? status = null)
    {
        var query = _context.Orders
            .AsNoTracking()
            .Include(o => o.Customer)
            .Include(o => o.Table)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.OrderDetailOptions)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(o => o.Status == status);
        }

        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return orders.Select(MapToDto);
    }

    public async Task<OrderResponseDto?> UpdateStatusAsync(Guid orderId, string newStatus)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var order = await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) return null;

            // Validate Order State Machine
            ValidateStateTransition(order.Status, newStatus);

            // Nếu Staff/Admin chủ động hủy đơn khi đơn đã Paid -> Hoàn tiền & trả kho
            if (newStatus == "Cancelled" && order.Status == "Paid")
            {
                await RestoreStockAndRefundAsync(order, "Hoàn tiền do quán hủy đơn");
            }

            order.Status = newStatus;
            order.UpdatedAt = DateTime.UtcNow;

            // Nếu đơn tại quán hoàn tất hoặc bị hủy -> Giải phóng bàn nếu không còn đơn active nào khác tại bàn đó
            if ((newStatus == "Completed" || newStatus == "Cancelled") && order.Table != null)
            {
                var hasOtherActiveOrders = await _context.Orders.AnyAsync(o =>
                    o.TableId == order.TableId &&
                    o.Id != order.Id &&
                    (o.Status == "Paid" || o.Status == "Preparing" || o.Status == "Ready"));

                if (!hasOtherActiveOrders)
                {
                    order.Table.Status = "Available";
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            var responseDto = await GetByIdAsync(order.Id)!;

            // Bắn SignalR báo tiến độ làm món cho khách hàng & KDS
            await _hubContext.Clients.Group($"Order_{order.Id}")
                .SendAsync("OrderStatusUpdated", responseDto);
            await _hubContext.Clients.Group("KitchenStaff")
                .SendAsync("OrderStatusUpdated", responseDto);

            return responseDto;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<OrderResponseDto?> CancelMyOrderAsync(Guid customerId, Guid orderId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var order = await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) return null;

            if (order.CustomerId != customerId)
                throw new BadHttpRequestException("Bạn không có quyền thao tác trên đơn hàng này.");

            // Quy tắc vàng chống bom hàng: Chỉ cho hủy khi quán chưa bấm Preparing
            if (order.Status != "PendingPayment" && order.Status != "Paid")
            {
                throw new BadHttpRequestException(
                    $"Không thể hủy đơn vì đơn hàng đang ở trạng thái '{order.Status}' (Barista đã tiếp nhận pha chế).");
            }

            if (order.Status == "Paid")
            {
                await RestoreStockAndRefundAsync(order, "Hoàn tiền tự động 100% do khách hủy đơn trước khi pha chế");
            }

            order.Status = "Cancelled";
            order.UpdatedAt = DateTime.UtcNow;

            if (order.Table != null)
            {
                var hasOtherActiveOrders = await _context.Orders.AnyAsync(o =>
                    o.TableId == order.TableId &&
                    o.Id != order.Id &&
                    (o.Status == "Paid" || o.Status == "Preparing" || o.Status == "Ready"));

                if (!hasOtherActiveOrders)
                {
                    order.Table.Status = "Available";
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            var responseDto = await GetByIdAsync(order.Id)!;

            await _hubContext.Clients.Group("KitchenStaff")
                .SendAsync("OrderCancelled", responseDto);

            return responseDto;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task RestoreStockAndRefundAsync(Order order, string reasonPrefix)
    {
        // Cộng lại tồn kho
        foreach (var detail in order.OrderDetails)
        {
            if (detail.Product != null)
            {
                detail.Product.StockQuantity += detail.Quantity;
            }
        }

        // Hoàn tiền vào ví khách hàng
        if (order.CustomerId.HasValue && order.TotalAmount > 0)
        {
            var shortCode = order.Id.ToString("N")[..6].ToUpper();
            await _walletService.RefundForOrderAsync(
                order.CustomerId.Value,
                order.Id,
                order.TotalAmount,
                $"{reasonPrefix} #{shortCode}");
        }
    }

    private static void ValidateStateTransition(string currentStatus, string nextStatus)
    {
        if (currentStatus == "Completed" || currentStatus == "Cancelled")
            throw new BadHttpRequestException($"Đơn hàng đã ở trạng thái cuối ({currentStatus}), không thể thay đổi.");

        var isValid = (currentStatus, nextStatus) switch
        {
            ("Paid", "Preparing") => true,
            ("Paid", "Cancelled") => true,
            ("Preparing", "Ready") => true,
            ("Ready", "Completed") => true,
            _ => false
        };

        if (!isValid)
        {
            throw new BadHttpRequestException($"Chuyển trạng thái không hợp lệ từ '{currentStatus}' sang '{nextStatus}'.");
        }
    }

    private static OrderResponseDto MapToDto(Order o)
    {
        return new OrderResponseDto
        {
            Id = o.Id,
            CustomerId = o.CustomerId,
            CustomerName = o.Customer?.FullName,
            TableId = o.TableId,
            TableNumber = o.Table?.TableNumber,
            TableName = o.Table?.TableName,
            OrderType = o.OrderType,
            TotalAmount = o.TotalAmount,
            Status = o.Status,
            ScheduledPickupTime = o.ScheduledPickupTime,
            Note = o.Note,
            CreatedAt = o.CreatedAt,
            UpdatedAt = o.UpdatedAt,
            Items = o.OrderDetails.Select(od =>
            {
                var optionsTotal = od.OrderDetailOptions.Sum(opt => opt.AdditionalPriceSnapshot);
                return new OrderDetailResponseDto
                {
                    Id = od.Id,
                    ProductId = od.ProductId,
                    ProductName = od.Product?.Name ?? "Sản phẩm",
                    ProductImageUrl = od.Product?.ImageUrl,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    SubTotal = (od.UnitPrice + optionsTotal) * od.Quantity,
                    Options = od.OrderDetailOptions.Select(opt => new OrderDetailOptionResponseDto
                    {
                        OptionId = opt.OptionId,
                        OptionName = opt.OptionNameSnapshot,
                        AdditionalPrice = opt.AdditionalPriceSnapshot
                    }).ToList()
                };
            }).ToList()
        };
    }
}