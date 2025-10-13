using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace melodia.Entities;

public class RecordLabel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; } 
    public string Email { get; set; } 
    public string PhoneNumber { get; set; }
    
    public City City { get; set; }
    public long CityId { get; set; }
    
    public List<Artist> Artists { get; set; }
}