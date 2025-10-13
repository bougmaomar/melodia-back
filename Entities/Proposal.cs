using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using melodia.Entities;

namespace melodia_api.Entities;

public class Proposal
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public long ArtistId { get; set; }
    public Artist Artist { get; set; }

    public long SongId { get; set; }
    public Song Song { get; set; }

    public long RadioStationId { get; set; }
    public RadioStation RadioStation { get; set; }

    public ProposalStatus Status { get; set; } = ProposalStatus.Pending;

    public DateTime ProposalDate { get; set; } = DateTime.UtcNow;
    public string? ProposalDescription { get; set; }
}