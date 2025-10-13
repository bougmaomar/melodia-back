using System.ComponentModel.DataAnnotations;

namespace melodia_api.Models.SongWriter;

public class SongWriterViewDto
{
    public long Id { get; set; }
    [Required] public long SongId { get; set; }
    public string SongName { get; set; }
    [Required] public long WritterId { get; set; }
    public string WritterName { get; set; }
}