using System.ComponentModel.DataAnnotations;

namespace melodia_api.Models.SongWriter;

public class SongWriterUpdateDto
{
    public long Id { get; set; }
    [Required] public long SongId { get; set; }
    [Required] public long WritterId { get; set; }
}