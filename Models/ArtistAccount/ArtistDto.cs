namespace melodia_api.Models.ArtistAccount;

public class ArtistDto
{
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Name { get; set; }
    public DateTime CareerStartDate { get; set; }
    public bool Active { get; set; }
    public string AccountId { get; set; }
}