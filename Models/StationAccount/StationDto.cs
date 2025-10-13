namespace melodia_api.Models.StationAccount
{
    public class StationDto
    {
        public long Id { get; set; }
        public string StationName { get; set; }
        public DateTime FoundationDate { get; set; }
        public bool Active { get; set; }
        public string Status { get; set; }
        public string AccountId { get; set; }
    }
}
