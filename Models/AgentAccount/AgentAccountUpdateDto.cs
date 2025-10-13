using System.ComponentModel.DataAnnotations;

namespace melodia_api.Models.AgentAccount;

public class AgentAccountUpdateDto
{
    public string accountId { get; set; }  
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string? WebSite { get; set; }
    public string? Bio {  get; set; }
    public long AgentId { get; set; }
    public long CityId { get; set; }
    public IFormFile? PhotoProfile { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime CareerStartDate { get; set; }
}