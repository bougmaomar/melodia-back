using System.ComponentModel.DataAnnotations;

namespace melodia_api.Models.Role;

public class RoleCreateDto
{
    [Required] public string Name { get; set; }
    public bool Active { get; set; }
}