using Masa.BuildingBlocks.Data;
using Microsoft.AspNetCore.SignalR;

namespace Gateway.Hubs;

public class LoggerHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        if (Context.GetHttpContext()!.Request.Query.TryGetValue("access_token", out var token))
        {
            await base.OnConnectedAsync();
            return;
        }
        throw new Exception("token is null");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }

    public async Task GetLogger(string logger)
    {
        try
        {
            if (GatewayApp.LoggerQueue.TryDequeue(out var value))
            {
                await Clients.Client(Context.ConnectionId).SendAsync("logger", value);
            }
        }
        catch (Exception)
        {

            throw;
        }
    }
}
