using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace melodia_api.Entities;

[Table("sections")]
public class Section
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public string Label { get; set; }
    public Section ParentSection { get; set; }
    public long? ParentSectionId { get; set; }

    public List<Section> SubSections { get; set; }
    public List<Access> Accesses { get; set; }
}