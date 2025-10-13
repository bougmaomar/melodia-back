using System.ComponentModel.DataAnnotations;

namespace melodia_api.Models.StationType;

public class StationTypeCreateDto
{
    [Required] public string Name { get; set; }
}