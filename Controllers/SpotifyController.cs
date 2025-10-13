using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;

namespace melodia_api.Controllers;

[Route("api/spotify")]
[ApiController]
public class SpotifyController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly string clientId = "0efc8e58c43143c8b92c4ace7f112713";
    private readonly string clientSecret = "6cf0e541a93d45d9b8b5b618fa516274";

    public SpotifyController()
    {
        _httpClient = new HttpClient();
    }
    [HttpGet("token")]
    public async Task<IActionResult> GetSpotifyToken()
    {
        try
        {
            var authHeader = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));

            var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
            request.Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            });

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(new { error = "Failed to fetch Spotify token", details = content });
            }

            return Ok(content);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Internal server error", details = ex.Message });
        }
    }
    
    
}
