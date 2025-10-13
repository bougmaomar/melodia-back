using melodia_api.Entities;
using Microsoft.AspNetCore.Identity;

namespace melodia.Entities;

public class Role : IdentityRole
{
    public bool Active { get; set; } = true;
    
    public List<AccountRole> AccountRoles { get; set; }
    public List<Access> Accesses { get; set; }
}