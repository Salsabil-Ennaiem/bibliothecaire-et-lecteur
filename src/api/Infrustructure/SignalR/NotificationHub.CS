using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.SignalR;

public class NotificationHub : Hub
{
    public async Task SubscribeUser(string userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, userId);
    }

    public async Task UnsubscribeUser(string userId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
    }
}
