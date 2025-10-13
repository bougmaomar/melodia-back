namespace melodia_api.Models;

public class Token
{
    public string Hash { get; set; }
    public DateTime ExpiresAt { get; set; }
}