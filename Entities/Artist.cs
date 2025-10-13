using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using melodia_api.Entities;

namespace melodia.Entities;

[Table("artists")]
public class Artist
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public string Name { get; set; } 
    public string? PhotoProfile { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Bio { get; set; }
    public string? Google { get; set; }
    public string? Facebook {  get; set; }
    public string? Instagram { get; set; }
    public string? Youtube { get; set; }
    public string? Spotify { get; set; }
    public DateTime CareerStartDate { get; set; }
    public bool Active { get; set; } = true;
    
    public long? AgentId { get; set; }
    public Agent Agent { get; set; }
    
    public long? CityId { get; set; }
    public City City { get; set; }
    
    public Account Account { get; set; }
    
    public List<Album> Albums { get; set; }
    public List<SongArtist> SongArtists { get; set; }
    public List<AlbumArtist> AlbumArtists { get; set; }
    public List<ArtistAgent> ArtistAgents { get; set; }
    
    public List<Proposal> Proposals { get; set; }
}