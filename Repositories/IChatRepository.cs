using melodia_api.Entities;

namespace melodia_api.Repositories
{
    public interface IChatRepository
    {
        Task<Discussion> StartDiscussionAsync(string stationId, string artistId);
        Task<Message> SaveMessageAsync(long discussionId, string senderId, string message);
        public Task<List<Discussion>> GetDiscussionsByEmail(string email);
        Task<List<Message>> GetDiscussionMessagesAsync(long discussionId);
        public Task<string> GetUserRole(string email);
        public Task<string> GetUserName(string email);
        Task CloseDiscussionAsync(long discussionId);
        Task<bool> IsChatAllowedAsync(string stationId, string artistId);
        public Task<bool> MarkMessageAsReadAsync(long discussionId, string email);
        public Task<Message> UpdateMessageAsync(long messageId, string message);
        public Task<bool> DeleteMessageAsync(long messageId);
        public Task<int> GetTotalMessages(string email);
        public Task<int> GetUnreadMessages(string email);

        //Task<List<Message>> GetMessageHistoryAsync(string userEmail1, string userEmail2);
    }
}
