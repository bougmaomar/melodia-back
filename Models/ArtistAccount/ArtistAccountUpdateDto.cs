using System.ComponentModel.DataAnnotations;

namespace melodia_api.Models.ArtistAccount;

public class ArtistAccountUpdateDto
{
    public string accountId { get; set; }  
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public long? AgentId { get; set; }
    public long ArtistId { get; set; }
    public long CityId { get; set; }
    public IFormFile? PhotoProfile { get; set; }
    public string? Bio { get; set; }
    public string? Google { get; set; }
    public string? Facebook { get; set; }
    public string? Instagram { get; set; }
    public string? Youtube { get; set; }
    public string? Spotify { get; set; }
    public string Name { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime CareerStartDate { get; set; }
}