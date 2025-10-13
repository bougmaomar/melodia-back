using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace melodia_api.Entities;

public class SectionIncluded
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public DateTime DateAdded { get; set; }
    public DateTime? DateRemoved { get; set; }
    public bool Active { get; set; } = true;

    public long SectionId { get; set; }
    public Section Section { get; set; }

    public long PlanId { get; set; }
    public Plan Plan { get; set; }
}