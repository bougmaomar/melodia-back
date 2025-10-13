using System.ComponentModel.DataAnnotations;

namespace melodia_api.Models.StationAccount
{
    public class StationAccountChangePasswordDto
    {
        [Required] public string Email { get; set; }
        [Required] public string CurrentPassword { get; set; }
        [Required] public string NewPassword { get; set; }
    }
}
