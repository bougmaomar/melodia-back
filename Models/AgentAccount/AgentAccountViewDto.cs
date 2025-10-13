using melodia_api.Models.Role;

namespace melodia_api.Models.AgentAccount;

public class AgentAccountViewDto
{
    public string Id { get; set; }
    public string? PhotoProfile {  get; set; }
    public string AgentRealName { get; set; }
    public DateTime CareerStartDate { get; set; }
    public string Email { get; set; }
    public int ArtistsNum { get; set; }
    public string Bio {  get; set; }
    public string? WebSite {  get; set; }
    public string Status { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public long AgentId { get; set; }
    public long? CityId { get; set; }
    public DateTime? LastLogin { get; set; }
    public List<string> ArtistNames { get; set; }
    public bool Active { get; set; }
}