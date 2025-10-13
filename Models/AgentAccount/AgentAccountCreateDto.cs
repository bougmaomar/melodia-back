using System.ComponentModel.DataAnnotations;

namespace melodia_api.Models.AgentAccount;

public class AgentAccountCreateDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    public string UserName { get; set; }
    public string? WebSite {  get; set; }
    public DateTime CareerStartDate { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}