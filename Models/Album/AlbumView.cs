
namespace melodia_api.Models.Album
{
    public class AlbumView
    {
        public long Id { get; set; }
        public string CoverImage { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; } = true;
        public DateTime ReleaseDate { get; set; }
        public TimeSpan TotalDuration { get; set; }
        public string AlbumTypeId { get; set; }
        public List<string> ArtistIds { get; set; }
    }
}
