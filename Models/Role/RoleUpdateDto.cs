using System.ComponentModel.DataAnnotations;

namespace melodia_api.Models.Role;

public class RoleUpdateDto
{
    [Required] public string Id { get; set; }
    [Required] public string Name { get; set; }
}