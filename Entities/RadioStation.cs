using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using melodia_api.Entities;

namespace melodia.Entities;

public class RadioStation
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public string StationName { get; set; }
    public string Frequency { get; set; }
    public string? WebSite { get; set; }
    public string StationOwner { get; set; }
    
    public string? Description { get; set; }
    public string? Logo { get; set; }
    public DateTime FoundationDate { get; set; }
    public string PhoneNumber { get; set; }
    public Account Account { get; set; }
    
    public City City { get; set; }
    public long? CityId { get; set; }
    
    public StationType StationType { get; set; }
    public long StationTypeId { get; set; }
    public List<RadioStationLanguage> StationLanguages { get; set; }
    public List<RadioStationMusicFormat> StationMusicFormats { get; set; }
    public List<Programme> Programmes { get; set; }
    public List<Employee> Employees { get; set; }
    
    public List<Proposal> Proposals { get; set; }
    
    public string? Status { get; set; }
    public bool Active { get; set; } = true;
}