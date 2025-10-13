using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace melodia.Entities;

[Table("songcomposers")]
public class SongComposer
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public long SongId { get; set; }
    public Song Song { get; set; }
    
    public long ComposerId { get; set; }
    public Composer Composer { get; set; }
    
    public bool Active { get; set; } = true;
}