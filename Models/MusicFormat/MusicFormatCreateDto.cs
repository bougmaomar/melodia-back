using System.ComponentModel.DataAnnotations;

namespace melodia_api.Models.MusicFormat
{
    public class MusicFormatCreateDto
    {
        [Required] public string Name { get; set; }
    }
}
