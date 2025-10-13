namespace melodia_api.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public string SenderEmail { get; set; }
        public string RecipientEmail { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }

        public long DiscussionId { get; set; }
        public Discussion Discussion { get; set; }
    }
}
