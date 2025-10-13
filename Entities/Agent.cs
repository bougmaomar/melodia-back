using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using melodia_api.Entities;

namespace melodia.Entities;

[Table("agents")]
public class Agent
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    [Required] public string FirstName { get; set; }
    [Required] public string LastName { get; set; }
    public DateTime CareerStartDate { get; set; }
    public string? PhotoProfile {  get; set; }
    public string PhoneNumber { get; set; }
    
    public string? Bio {  get; set; }
    public City City { get; set; }
    public long? CityId { get; set; }
    public int ArtistsNum { get; set; }
    public string? WebSite { get; set; }
    public bool Active { get; set; } = true;
    public string Status { get; set; }
    public Account Account { get; set; }
    
    public List<ArtistAgent> ArtistAgents { get; set; }
}