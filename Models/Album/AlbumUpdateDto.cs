using melodia.Entities;

namespace melodia_api.Models.Album
{
    public class AlbumUpdateDto
    { 
        public IFormFile? CoverImage { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime ReleaseDate { get; set; }
        public long AlbumTypeId { get; set; }
        public List<long> ArtistIds { get; set; }
    }
}
