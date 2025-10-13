using melodia_api.Models.Employee;
using melodia.Entities;

namespace melodia_api.Models.StationAccount
{
    public class StationAccountViewDto
    {
        public string Id { get; set; }
        public string? Logo { get; set; }
        public string StationName { get; set; }
        public DateTime FoundationDate { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? Description { get; set; }
        public string Password { get; set; }
        public long StationId { get; set; }
        public long? CityId { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool Active { get; set; }
        public string Frequency { get; set; }
        public string? WebSite { get; set; }
        public string Status { get; set; }
        public string StationOwner { get; set; }
        public string StationTypeName { get; set; }
        public long StationTypeId { get; set; }
        public List<string> StationLanguages { get; set; }
        public List<string> StationMusicFormats { get; set; }
        public List<string> ProgramNames { get; set; }
        public List<EmployeeViewDto> Employees { get; set; }
    }
}
