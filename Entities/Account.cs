using melodia_api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace melodia.Entities;

[Index(nameof(Email), IsUnique = true)]
public class Account : IdentityUser
{
    public DateTime? LastLogin { get; set; }
    public bool Active { get; set; } = true;
    public bool IsApproved { get; set; } = false;
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    
    public List<AccountRole> AccountRoles { get; set; }

    public long? AgentId { get; set; }
    public Agent Agent { get; set; }

    public Artist Artist { get; set; }
    public long? ArtistId { get; set; }
    
    public RadioStation RadioStation { get; set; }
    public long? RadioStationId { get; set; }
    
    public List<FavoriteSongs> FavoriteSongs { get; set;}

}