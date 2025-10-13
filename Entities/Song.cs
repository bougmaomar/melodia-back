using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using melodia_api.Entities;
using melodia.Entities;

namespace melodia.Entities;

[Table("songs")]
public class Song
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public string Title { get; set; }
    [Required] public DateTime ReleaseDate { get; set; }
    public DateTime? PlatformReleaseDate { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
    
    public string CodeISRC { get; set; }
    public GenreMusic GenreMusic { get; set; }
    public long GenreMusicId { get; set; }
    //public Language Language { get; set; }
    //public long LanguageId { get; set; }
    [Required] public string Lyrics { get; set; }
    public string Duration { get; set; }
    
    public bool IsMapleMusic { get; set; }
    public bool IsMapleArtist { get; set; }
    public bool IsMaplePerformance { get; set; }
    public bool IsMapleLyrics { get; set; }
    public string? YouTube { get; set; }
    public string? Spotify { get; set; }
     
    public string Mp3FilePath { get; set; }
    public string WavFilePath { get; set; }
    public string CoverImagePath { get; set; }
    
    public long? AlbumId { get; set; } 
    public Album Album { get; set; }
    
    public List<SongArtist> SongArtists { get; set; }
    public List<SongLanguages> SongLanguages { get; set; }
    public List<SongCROwner> SongCrOwners { get; set; }
    public List<SongWriter> SongWriters { get; set; }
    public List<SongComposer> SongComposers { get; set; }
    public List<SongPOwner> SongPOwners { get; set; }
    public List<Proposal> Proposals { get; set; }

    public bool Active { get; set; } = true;

    public bool SystemManage { get; set; } = true;
    
    public List<FavoriteSongs> FavoriteSongs { get; set;}
} 