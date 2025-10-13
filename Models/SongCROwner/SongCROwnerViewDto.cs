using System.ComponentModel.DataAnnotations;

namespace melodia_api.Models.SongCROwner;

public class SongCROwnerViewDto
{
    public long Id { get; set; }
    [Required] public long SongId { get; set; }
    public string SongName { get; set; }
    [Required] public long CROwnerId { get; set; }
    public string CROwnerName { get; set; }
}