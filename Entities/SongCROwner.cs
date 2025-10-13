using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace melodia.Entities;

[Table("songcrowners")]
public class SongCROwner
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public long SongId { get; set; } 
    public Song Song { get; set; }
    
    public long CROwnerId { get; set; }
    public CROwner CrOwner { get; set; }
    
    public bool Active { get; set; } = true;
}