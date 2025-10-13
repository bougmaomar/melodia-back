using System.ComponentModel.DataAnnotations;

namespace melodia_api.Models.AgentAccount;

public class AgentAccountChangeRoleDto
{
    [Required] public string Email { get; set; }
    [Required] public string RoleName { get; set; }
}