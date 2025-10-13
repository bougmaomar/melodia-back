using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using melodia_api.Chat;
using melodia_api.Repositories;

namespace melodia_api.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly IChatRepository _chatRepository;

        public ChatController(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartChat(string userEmail1, string userEmail2)
        {
            var discussion = await _chatRepository.StartDiscussionAsync(userEmail1, userEmail2);
            var lastMessage = discussion.Messages?.LastOrDefault();
            var unreadCount = discussion.Messages?.Count(m => m.SenderEmail!= userEmail1 && !m.IsRead) ?? 0;

            // Get the correct user role asynchronously
            var userEmail = discussion.InitiatorEmail != userEmail1 ? discussion.InitiatorEmail : discussion.RecipientEmail;
            var userRole = await _chatRepository.GetUserRole(userEmail);
            var userName = await _chatRepository.GetUserName(userEmail);
            var formattedDiscussion = new
            {
                discussion.Id,
                Name = userName,
                Email = userEmail,
                LastMessage = lastMessage?.Content,
                Time = lastMessage?.Timestamp,
                Unread = unreadCount,
                Role = userRole, // Now correctly awaited
                Seen = unreadCount == 0
            };
            return Ok(formattedDiscussion);
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage(long discussionId, string senderEmail, string message)
        {
            var msg = await _chatRepository.SaveMessageAsync(discussionId, senderEmail, message);
            return Ok(msg);
        }

        [HttpPost("close")]
        public async Task<IActionResult> CloseChat(long discussionId)
        {
            await _chatRepository.CloseDiscussionAsync(discussionId);
            return Ok(new { Message = "Chat closed successfully!" });
        }

        [HttpGet("discussions")]
        public async Task<IActionResult> GetDiscussions(string email)
        {
            var discussions = await _chatRepository.GetDiscussionsByEmail(email);
            return Ok(discussions);
        }
        
        [HttpGet("last_discussion_message")]
        public async Task<IActionResult> GetLastDiscussions(string email)
        {
            var discussions = await _chatRepository.GetDiscussionsByEmail(email);

            if (discussions == null)
                return NotFound("No discussions found for this email.");

            var detailedDiscussions = new List<object>();

            foreach (var d in discussions)
            {
                var lastMessage = d.Messages?.LastOrDefault();
                var unreadCount = d.Messages?.Count(m => m.SenderEmail!= email && !m.IsRead) ?? 0;

                // Get the correct user role asynchronously
                var userEmail = d.InitiatorEmail != email ? d.InitiatorEmail : d.RecipientEmail;
                var userRole = await _chatRepository.GetUserRole(userEmail);
                var userName = await _chatRepository.GetUserName(userEmail);

                detailedDiscussions.Add(new
                {
                    d.Id,
                    Name = userName,
                    Email = userEmail,
                    LastMessage = lastMessage?.Content,
                    Time = lastMessage?.Timestamp,
                    Unread = unreadCount,
                    Role = userRole, // Now correctly awaited
                    Seen = unreadCount == 0
                });
            }

            return Ok(detailedDiscussions);
        }


        [HttpGet("messages")]
        public async Task<IActionResult> GetMessages(long discussionId)
        {
            var messages = await _chatRepository.GetDiscussionMessagesAsync(discussionId);
            return Ok(messages);
        }

        [HttpGet("total_messages")]
        public async Task<IActionResult> GetTotalMessages(string email)
        {
            var messages = await _chatRepository.GetTotalMessages(email);
            return Ok(messages);
        }

        [HttpGet("total_unread_messages")]
        public async Task<IActionResult> GetTotalUnreadMessages(string email)
        {
            var messages = await _chatRepository.GetUnreadMessages(email);
            return Ok(messages);
        }

        [HttpPut("mask_as_read")]
        public async Task<IActionResult> MaskMessagesAsRead(long discussionId, string email)
        {
            var response = await _chatRepository.MarkMessageAsReadAsync(discussionId, email);
            return Ok(response);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateMessage(long messageId, string message)
        {
            var updatedMessage = await _chatRepository.UpdateMessageAsync(messageId, message);
            return Ok(updatedMessage);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteMessage(long messageId)
        {
            var deletedMessage = await _chatRepository.DeleteMessageAsync(messageId);
            return Ok(deletedMessage);
        }
    }
}

