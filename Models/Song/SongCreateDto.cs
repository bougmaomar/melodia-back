namespace melodia_api.Models.Song;

public class SongCreateDto
{
    public string Title { get; set; }
    public DateTime ReleaseDate { get; set; }
    public DateTime? PlatformReleaseDate { get; set; }
    public long GenreMusicId { get; set; }
    //public long LanguageId { get; set; }
    public string Lyrics { get; set; }
    public bool IsMapleMusic { get; set; }
    public bool IsMapleArtist { get; set; }
    public bool IsMaplePerformance { get; set; }
    public bool IsMapleLyrics { get; set; }
    public List<long> CopyrightOwnerIds { get; set; }
    public List<long> WriterIds { get; set; }
    public List<long> ComposerIds { get; set; }
    public List<long> ArtistIds { get; set; }
    public List<long> LanguageIds { get; set; }
    public long? AlbumId { get; set; }
    public string CodeISRC { get; set; }

    public string? YouTube { get; set; }
    public string? Spotify { get; set; }
    public bool SystemManage { get; set; }
    
    public IFormFile AudioFile { get; set; }
    public IFormFile WavFile { get; set; }
    public IFormFile CoverImage { get; set; }
}
