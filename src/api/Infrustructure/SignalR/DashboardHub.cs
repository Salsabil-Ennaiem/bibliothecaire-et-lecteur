using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.SignalR;

public class dashboardHub : Hub
{
    public async Task SubscribeUser()
    {
               await Clients.Caller.SendAsync("Connected", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public async Task UnsubscribeUser(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}
