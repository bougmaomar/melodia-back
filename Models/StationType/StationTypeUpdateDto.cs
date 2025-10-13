using System.ComponentModel.DataAnnotations;

namespace melodia_api.Models.StationType;

public class StationTypeUpdateDto
{
    [Required] public long Id { get; set; }
    [Required] public string Name { get; set; }
}