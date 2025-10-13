using melodia.Entities;

namespace melodia_api.Entities
{
    public class Discussion
    {
        public long Id { get; set; }
        public bool IsAllowed { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime CloseAt { get; set; }
        public string InitiatorEmail { get; set; }
        public string RecipientEmail { get; set; }
        public List<Message> Messages { get; set; }
    }
}
