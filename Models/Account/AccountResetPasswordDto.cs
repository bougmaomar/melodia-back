using System.ComponentModel.DataAnnotations;

namespace melodia_api.Models.Account;

public class AccountResetPasswordDto
{
    [Required] public string NewPassword { get; set; }
}