using System.ComponentModel.DataAnnotations;

namespace melodia_api.Models.StationAccount
{
    public class StationAccountChangeRoleDto
    {
        [Required] public string Email { get; set; }
        [Required] public string RoleName { get; set; }
    }
}
