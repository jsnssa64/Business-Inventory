using Microsoft.AspNetCore.SignalR;

namespace NotificationService.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendAllMessage(string message) =>
            await Clients.All.SendAsync("messageReceived", message);

        public async Task SendSingleClientMessage(string connectionId, string message) =>
            await Clients.Client(connectionId).SendAsync("messageReceived", message);

        public async Task SendMultipleClientsMessage(IEnumerable<string> connectionIds, string message) =>
            await Clients.Clients(connectionIds).SendAsync("messageReceived", message);

        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }
    }
}
