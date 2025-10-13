using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace melodia.Entities;

[Table("songartists")]
public class SongArtist
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public long SongId { get; set; }
    public Song Song { get; set; }
    
    public long ArtistId { get; set; }
    public Artist Artist { get; set; }
    
    public bool Active { get; set; } = true;
}