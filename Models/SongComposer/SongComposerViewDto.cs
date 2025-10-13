using System.ComponentModel.DataAnnotations;

namespace melodia_api.Models.SongComposer;

public class SongComposerViewDto
{
    public long Id { get; set; }
    [Required] public long SongId { get; set; }
    public string SongName { get; set; }
    [Required] public long ComposerId { get; set; }
    public string ComposerName { get; set; }
}