namespace melodia_api.Models.AgentAccount;

public class AgentDto
{
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime CareerStartDate { get; set; }
    public bool Active { get; set; }
    public string Status { get; set; }
    public string AccountId { get; set; }
}