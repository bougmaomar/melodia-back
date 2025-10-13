using System.ComponentModel.DataAnnotations;

namespace melodia_api.Models.AgentAccount;

public class AgentAccountChangePasswordDto
{
    [Required] public string Email { get; set; }
    [Required] public string CurrentPassword { get; set; }
    [Required] public string NewPassword { get; set; }
}