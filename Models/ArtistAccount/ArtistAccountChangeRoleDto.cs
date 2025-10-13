using System.ComponentModel.DataAnnotations;

namespace melodia_api.Models.ArtistAccount;

public class ArtistAccountChangeRoleDto
{
    [Required] public string Email { get; set; }
    [Required] public string RoleName { get; set; }
}