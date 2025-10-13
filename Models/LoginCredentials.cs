using System.ComponentModel.DataAnnotations;

namespace melodia_api.Models;

public class LoginCredentials
{
    [Required] public string Email { get; set; }
    [Required] public string Password { get; set; }
}