using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace melodia.Entities;

[Table("albums")]
public class Album
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public string CoverImage { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool Active { get; set; } = true;
    public DateTime ReleaseDate { get; set; }
    public TimeSpan TotalDuration { get; set; }
    
    public AlbumType AlbumType { get; set; }
    public long AlbumTypeId { get; set; }
    
    public List<Song> Songs { get; set; }
    public List<AlbumArtist> AlbumArtists { get; set; }
}