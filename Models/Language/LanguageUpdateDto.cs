using System.ComponentModel.DataAnnotations;

namespace melodia_api.Models.Language
{
    public class LanguageUpdateDto
    {
        [Required] public long Id { get; set; }
        [Required] public string Label { get; set; }
    }
}
