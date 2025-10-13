using System.ComponentModel.DataAnnotations;

namespace melodia_api.Models.SongComposer;

public class SongComposerCreateDto
{
    [Required] public long SongId { get; set; }
    [Required] public long ComposerId { get; set; }
}