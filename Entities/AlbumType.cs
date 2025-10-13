using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace melodia.Entities;

public class AlbumType
{
    // e.g., "Studio", "Live", "Compilation"
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; } 
    [Required] public string Name { get; set; }
    public bool Active { get; set; } = true;
}