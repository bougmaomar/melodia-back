namespace melodia_api.Models.Song
{
    public class SongView
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public DateTime ReleaseDate { get; set; }
        public DateTime PlatformReleaseDate { get; set; }

        public List<long> ArtistIds { get; set; }
        public string CodeISRC { get; set; }

        public long AlbumId { get; set; }
        public long GenreMusicId { get; set; }
        //public long LanguageId { get; set; }
        public string Lyrics { get; set; }

        // Informations de MAPL
        public bool IsMapleMusic { get; set; }
        public bool IsMapleArtist { get; set; }
        public bool IsMaplePerformance { get; set; }
        public bool IsMapleLyrics { get; set; }

        public List<long> LanguageIds { get; set; }
        public List<long> ComposersIds { get; set; }
        public List<long> WritersIds { get; set; }
        public List<long> CROwnersIds { get; set; }

        public string Mp3FilePath { get; set; }
        public string WavFilePath { get; set; }
        public string CoverImagePath { get; set; }

        public string YouTube { get; set; }
        public string Spotify { get; set; }
    }
}
