using System.Collections.Concurrent;
using melodia.Configurations;
using melodia_api.Chat;
using melodia_api.Entities;
using melodia_api.Models.Account;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace melodia_api.Repositories.Implementations
{

    public class ChatRepository : IChatRepository
    {
        private readonly MelodiaDbContext _db;
        private readonly IHubContext<ChatHub> _hubContext;

        private static readonly ConcurrentDictionary<string, string> activeChats = new();

        public ChatRepository(MelodiaDbContext context, IHubContext<ChatHub> hubContext)
        {
            _db = context;
            _hubContext = hubContext;
        }

        public async Task<Discussion> StartDiscussionAsync(string userEmail1, string userEmail2)
        {
            var existingDiscussion = await _db.Discussions
                .Include(d => d.Messages).FirstOrDefaultAsync(d => d.InitiatorEmail == userEmail1 && d.RecipientEmail == userEmail2 || d.InitiatorEmail == userEmail2 && d.RecipientEmail == userEmail1);

            if (existingDiscussion != null)
            {
                return existingDiscussion;
            }
            string existingEmail1 = await _db.Accounts
                .Where(a => a.Email == userEmail1)
                .Select(a => a.Email)
                .FirstOrDefaultAsync();
            string existingEmail2 = await _db.Accounts
                .Where(a => a.Email == userEmail2)
                .Select(a => a.Email)
                .FirstOrDefaultAsync();

            if (existingEmail1 == null) throw new Exception($"{existingEmail1} Account not found");
            if (existingEmail2 == null) throw new Exception($"{existingEmail2} Account not found");
            
            var discussion = new Discussion
            {
                InitiatorEmail = userEmail1,
                RecipientEmail = userEmail2,
                IsAllowed = true,
                StartAt = DateTime.UtcNow
            };

            _db.Discussions.Add(discussion);
            await _db.SaveChangesAsync();

            var groupName = $"{userEmail1}-{userEmail2}";
            await _hubContext.Clients.User(userEmail2).SendAsync("ChatOpened", $"User {userEmail1} started a chat with you.");
            await _hubContext.Groups.AddToGroupAsync(userEmail1, groupName);
            await _hubContext.Groups.AddToGroupAsync(userEmail2, groupName);
            var disc = await _db.Discussions.Include(d => d.Messages).FirstOrDefaultAsync(d => d.Id == discussion.Id);
            return disc;
        }

        public async Task<Message> SaveMessageAsync(long discussionId, string senderEmail, string messageContent)
        {
            var discussion = await _db.Discussions.FindAsync(discussionId);
            if (discussion == null || !discussion.IsAllowed)
            {
                throw new InvalidOperationException("Chat is not allowed or doesn't exist.");
            }
            if (discussion.InitiatorEmail != senderEmail && discussion.RecipientEmail != senderEmail)
            {
                throw new Exception("This Email doesn't exist in this discussion");
            }

                var recipient = " ";
                if (discussion.InitiatorEmail == senderEmail) recipient = discussion.RecipientEmail;
                else recipient = discussion.InitiatorEmail;


            var message = new Message
                {
                    DiscussionId = discussion.Id,
                    SenderEmail = senderEmail,
                    RecipientEmail = recipient,
                    Content = messageContent,
                    Timestamp = DateTime.UtcNow
                };

                _db.Messages.Add(message);
                await _db.SaveChangesAsync();

                var groupName = $"{discussion.InitiatorEmail}-{discussion.RecipientEmail}";
                await _hubContext.Clients.Group(groupName).SendAsync("ReceiveMessage", senderEmail, messageContent);
            return message;
            
        }

        public async Task<List<Discussion>> GetDiscussionsByEmail(string email)
        {
            var discussions = await _db.Discussions
                .Include(d => d.Messages)
                .Where(d => d.InitiatorEmail == email || d.RecipientEmail == email)
                .ToListAsync();
            return discussions;
        }

        public async Task<Message> UpdateMessageAsync(long messageId, string content)
        {
            var existingMessage = await _db.Messages.FirstOrDefaultAsync(m => m.Id == messageId);
            existingMessage.Content = content;
            existingMessage.Timestamp = DateTime.Now;
            _db.Messages.Update(existingMessage);
            await _db.SaveChangesAsync();
            
            return existingMessage;
        }

        public async Task<bool> DeleteMessageAsync(long messageId)
        {
            var existingMessage = await _db.Messages.FirstOrDefaultAsync(m => m.Id == messageId);
            _db.Messages.Remove(existingMessage);
            await _db.SaveChangesAsync();
            
            return existingMessage != null;
        }
        
        public async Task<string> GetUserRole(string email)
        {
            var user = await _db.Accounts.Include(a => a.AccountRoles).ThenInclude(aa => aa.Role).FirstOrDefaultAsync(r => r.Email == email);
            var userRole = user?.AccountRoles.Select(r => r.Role.Name);
            return userRole.FirstOrDefault();
        }

        public async Task<string> GetUserName(string email)
        {
            var user = await _db.Accounts.Where(u => u.Email == email).FirstOrDefaultAsync();
            return user.UserName;
        }
        
        public async Task<List<Message>> GetDiscussionMessagesAsync(long discussionId)
        {
            return await _db.Messages
                .Where(m => m.DiscussionId == discussionId)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
        }

        public async Task CloseDiscussionAsync(long discussionId)
        {
            var discussion = await _db.Discussions.FindAsync(discussionId);
            if (discussion == null)
            {
                throw new InvalidOperationException("Discussion not found.");
            }

            discussion.CloseAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            var groupName = $"{discussion.InitiatorEmail}-{discussion.RecipientEmail}";
            await _hubContext.Clients.Group(groupName).SendAsync("ChatClosed", "Chat has been closed.");
            await _hubContext.Groups.RemoveFromGroupAsync(discussion.InitiatorEmail, groupName);
            await _hubContext.Groups.RemoveFromGroupAsync(discussion.RecipientEmail, groupName);
        }
        
        public async Task<bool> IsChatAllowedAsync(string userEmail1, string userEmail2)
        {
            var discussion = await _db.Discussions
                .FirstOrDefaultAsync(
                d => d.InitiatorEmail == userEmail1 && d.RecipientEmail == userEmail2 || d.RecipientEmail == userEmail1 && d.InitiatorEmail == userEmail2 && d.CloseAt == null);

            return discussion != null && discussion.IsAllowed;
        }

        public async Task<bool> MarkMessageAsReadAsync(long discussionId, string email)
        {
            var messages = await _db.Messages.Where(m => m.DiscussionId == discussionId).ToListAsync();
            foreach (var message in messages)
            {
                if (message.SenderEmail == email) continue;
                message.IsRead = true;
            }
            _db.Messages.UpdateRange(messages);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetTotalMessages(string email)
        {
            var discussions = await GetDiscussionsByEmail(email);
            var messages = 0;
            foreach (var disc in discussions)
            {
                messages += disc.Messages.Count();
            }
            return messages;
        }

        public async Task<int> GetUnreadMessages(string email)
        {
            var discussions = await GetDiscussionsByEmail(email);
            var messages = 0;
            foreach (var disc in discussions)
            {
                messages += disc.Messages.Where(m => m.RecipientEmail == email && m.IsRead == false).Count();
            }
            return messages;
        }
    }
}
