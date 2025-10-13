using System.ComponentModel.DataAnnotations;

namespace melodia_api.Models.Language
{
    public class LanguageCreateDto
    {
        [Required] public string Label { get; set; }
    }
}
