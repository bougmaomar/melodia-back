using melodia.Entities;

namespace melodia_api.Entities
{
    public class SongPartner
    {
        public long SongId { get; set; }
        public Song Song { get; set; }
        public long ArtistId { get; set; }
        public Artist Artist { get; set; }
        public long? ComposerId { get; set; }
        public Composer Composer { get; set; }
        public long? WriterId { get; set; }
        public Writer Writer { get; set; }
    }
}
