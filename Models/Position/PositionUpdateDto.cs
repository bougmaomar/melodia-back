using System.ComponentModel.DataAnnotations;

namespace melodia_api.Models.Position
{
    public class PositionUpdateDto
    {
        [Required] public long Id { get; set; }
        [Required] public string Name { get; set; }
    }
}
