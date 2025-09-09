using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.SignalR;
public class DashboardHub : Hub
{
    public async Task JoinBiblioGroup(string biblioId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"biblio_{biblioId}");
        await Clients.Caller.SendAsync("JoinedGroup", $"biblio_{biblioId}");
    }

    public async Task LeaveBiblioGroup(string biblioId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"biblio_{biblioId}");
        await Clients.Caller.SendAsync("LeftGroup", $"biblio_{biblioId}");
    }

    public async Task RequestDashboardUpdate(string biblioId)
    {
        // Client can request immediate update
        await Clients.Group($"biblio_{biblioId}")
            .SendAsync("UpdateRequested", new { biblioId, requestedBy = Context.ConnectionId });
    }

    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("Connected", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}
