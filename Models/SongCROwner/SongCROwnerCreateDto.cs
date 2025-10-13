using System.ComponentModel.DataAnnotations;

namespace melodia_api.Models.SongCROwner;

public class SongCROwnerCreateDto
{
    [Required] public long SongId { get; set; }
    [Required] public long CROwnerId { get; set; }
}