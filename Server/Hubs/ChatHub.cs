using BlazorChat.Shared.Models;
using Microsoft.AspNetCore.SignalR;

namespace BlazorChat.Server.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(Message message)
        {
            var users = new[] { message.ToUserId, message.FromUserId };
            await Clients.Users(users).SendAsync("ReceiveMessage", message);
        }
    }
}
