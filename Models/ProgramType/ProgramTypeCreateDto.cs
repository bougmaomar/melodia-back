using System.ComponentModel.DataAnnotations;

namespace melodia_api.Models.ProgramType
{
    public class ProgramTypeCreateDto
    {
        [Required] public string Name { get; set; }
    }
}
