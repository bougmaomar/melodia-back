using DocumentFormat.OpenXml.Wordprocessing;

namespace melodia_api.Models.StationAccount
{
    public class StationAccountUpdateDto
    {
        public string accountId { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? Description { get; set; }
        public long StationId { get; set; }
        public long? CityId { get; set; }
        public IFormFile? Logo { get; set; }
        public string StationName { get; set; }
        public DateTime FoundationDate { get; set; }
        public string Frequency { get; set; }
        public string? WebSite { get; set; }
        public string StationOwner { get; set; }
        public long StationTypeId { get; set; }
    }
}
