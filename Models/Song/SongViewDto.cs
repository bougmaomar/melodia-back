namespace melodia_api.Models.Song;

public class SongViewDto
{
    public long Id { get; set; }
    public string Title { get; set; }
    public DateTime ReleaseDate { get; set; }
    public List<string> ArtistNames { get; set; }
    public List<string> ArtistEmails { get; set; }
    public string CodeISRC { get; set; }

    public string AlbumTitle { get; set; } 
    public string GenreMusicName { get; set; }
    //public string LanguageLabel { get; set; }
    public List<string> LanguageLabels { get; set; }
    public string Lyrics { get; set; }
    public TimeSpan Duration { get; set; }

    // Informations de MAPL
    public bool IsMapleMusic { get; set; }
    public bool IsMapleArtist { get; set; }
    public bool IsMaplePerformance { get; set; }
    public bool IsMapleLyrics { get; set; }
    
    public List<string> ComposersNames { get; set; }
    public List<string> WritersNames { get; set; }
    public List<string> CROwnersNames { get; set; }

    public string Mp3FilePath { get; set; }
    public string WavFilePath { get; set; }
    public string CoverImagePath { get; set; }
    
    public string YouTube { get; set; }
    public string Spotify { get; set; }
}
