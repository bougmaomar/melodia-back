namespace melodia.Entities;

public class AlbumArtist
{
    public long AlbumId { get; set; }
    public virtual Album Album { get; set; }

    public long ArtistId { get; set; }
    public virtual Artist Artist { get; set; }
}