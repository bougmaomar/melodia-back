using Microsoft.AspNetCore.Identity;

namespace melodia.Entities;

public class AccountRole : IdentityUserRole<string>
{
    public Account Account { get; set; }
    public Role Role { get; set; }
}