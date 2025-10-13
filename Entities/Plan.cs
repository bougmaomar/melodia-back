using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace melodia_api.Entities;

[Table("plans")]
public class Plan
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public string Label { get; set; }
    public string Description { get; set; }
    public decimal CurrentPrice { get; set; }
    public bool Monthly { get; set; }
    public bool IncludesWhiteLableing { get; set; }
    public int StandardUsersCount { get; set; }
    public decimal CostPerExtraUser { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public List<SectionIncluded> SectionIncludeds { get; set; }
}