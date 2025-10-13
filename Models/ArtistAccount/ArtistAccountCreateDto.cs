using System.ComponentModel.DataAnnotations;

namespace melodia_api.Models.ArtistAccount;

public class ArtistAccountCreateDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    public string Name { get; set; }
    
    public DateTime CareerStartDate { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}