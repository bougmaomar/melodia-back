using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace melodia.Entities;

[Table("songpowners")]
public class SongPOwner
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public long SongId { get; set; }
    public Song Song { get; set; }

    public long POwnerId { get; set; }
    public POwner POwner { get; set; }
    
    public bool Active { get; set; } = true;
}