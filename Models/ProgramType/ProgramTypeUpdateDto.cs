using System.ComponentModel.DataAnnotations;

namespace melodia_api.Models.ProgramType
{
    public class ProgramTypeUpdateDto
    {
        [Required] public long Id { get; set; }
        [Required] public string Name { get; set; }
    }
}
