using System.ComponentModel.DataAnnotations;

namespace melodia_api.Models.MusicFormat
{
    public class MusicFormatUpdateDto
    {
        [Required] public long Id { get; set; }
        [Required] public string Name { get; set; }
    }
}
