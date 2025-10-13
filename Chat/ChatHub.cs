using melodia.Configurations;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace melodia_api.Chat
{
    public class ChatHub : Hub
    {
        public async Task OpenChat(string senderId, string recipientId)
        {
            var groupName = $"{senderId}-{recipientId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.User(recipientId).SendAsync("ChatOpened", $"User {senderId} opened a chat with you.");
        }

        public async Task SendMessage(string groupId, string senderId, string message)
        {
            await Clients.Group(groupId).SendAsync("ReceiveMessage", senderId, message);
        }

        public async Task CloseChat(string senderId, string recipientId)
        {
            var groupName = $"{senderId}-{recipientId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.User(recipientId).SendAsync("ChatClosed", $"User {senderId} has closed the chat.");
        }
    }
}
