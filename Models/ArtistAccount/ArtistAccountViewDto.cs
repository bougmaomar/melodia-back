using System.Net.NetworkInformation;
using melodia_api.Models.Role;

namespace melodia_api.Models.ArtistAccount;

public class ArtistAccountViewDto
{
    public string Id { get; set; }
    public string Name { get; set; } 
    public string PhotoProfile {  get; set; }
    public string ArtistRealName { get; set; }
    public DateTime CareerStartDate { get; set; }
    public string Bio {  get; set; }
    public string Google { get; set; }
    public string Facebook { get; set; }
    public string Instagram { get; set; }
    public string Youtube { get; set; }
    public string Spotify { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public long ArtistId { get; set; }
    public long? CityId { get; set; }
    public long? AgentId {  get; set; }
    public DateTime? LastLogin { get; set; }
    
    public int NumberOfAlbums { get; set; }
    public int NumberOfSingles { get; set; }
    
    public int NumberOfAlbumSongs { get; set; }
    public bool Active { get; set; }
}