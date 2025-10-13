using melodia.Entities;

namespace melodia_api.Models.StationAccount
{
    public class StationAccountCreateDto
    {
        public string StationName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Frequency { get; set; }
        public string? WebSite { get; set; }
        public string StationOwner { get; set; }
        public DateTime FoundationDate { get; set; }
        public long? CityId { get; set; }
        public long StationTypeId { get; set; }
      
    }
}
