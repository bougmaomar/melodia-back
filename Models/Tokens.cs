
using DocumentFormat.OpenXml.Office.CoverPageProps;

namespace melodia_api.Models;

public class Tokens
{
    public Token? AccessToken { get; set; }
    public Token? RefreshToken { get; set; }
    public UserInfo? User { get; set; }
    public string? Status {  get; set; }
}