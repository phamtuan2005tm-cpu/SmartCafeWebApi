using Microsoft.AspNetCore.SignalR;

namespace SmartCafe.API.Hubs;

public class OrderHub : Hub
{
    // Nhân viên/Barista tham gia nhóm KDS để nhận đơn mới theo thời gian thực
    public async Task JoinKitchenGroup()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "KitchenStaff");
    }

    // Khách hàng tham gia phòng theo dõi riêng của đơn hàng vừa đặt
    public async Task JoinOrderGroup(string orderId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Order_{orderId}");
    }
}