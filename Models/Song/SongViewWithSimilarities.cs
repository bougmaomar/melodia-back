namespace melodia_api.Models.Song
{
    public class SongViewWithSimilarities
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public DateTime ReleaseDate { get; set; }

        public List<long> ArtistIds { get; set; }

        public string Lyrics { get; set; }

        public float SimilarityPercentage { get; set; }

    }
}
