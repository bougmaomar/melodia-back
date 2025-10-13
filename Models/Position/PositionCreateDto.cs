using System.ComponentModel.DataAnnotations;

namespace melodia_api.Models.Position
{
    public class PositionCreateDto
    {
        [Required] public string Name { get; set; }
    }
}
