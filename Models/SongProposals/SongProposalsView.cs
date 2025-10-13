
namespace melodia_api.Models.SongProposals;

public class SongProposalsView
{
    public long Id { get; set; }

    public long ArtistId { get; set; }
    public string ArtistName { get; set; }

    public long SongId { get; set; }
    public string SongName { get; set; }

    public long RadioStationId { get; set; }
    public string RadioStationName { get; set; }

    public string Status { get; set; }
}