using System.ComponentModel.DataAnnotations;

namespace melodia_api.Models.SongWriter;

public class WriterUpdateDto
{
    public long Id { get; set; } 
    [Required]  public string Name { get; set; }
}