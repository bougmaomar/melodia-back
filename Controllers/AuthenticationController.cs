using melodia_api.Models;
using melodia_api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace melodia_api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
	private readonly IAuthenticationRepository _repository;

	public AuthenticationController(IAuthenticationRepository repository)
	{
		_repository = repository;
	}

	[HttpPost("login")]
	public async Task<ActionResult<Tokens>> Authenticate([FromBody] LoginCredentials loginCredentials)
	{
		if (!ModelState.IsValid) return BadRequest("Bad Request");
		var tokens = await _repository.Authenticate(loginCredentials.Email, loginCredentials.Password);
		if (tokens == null) return NotFound();
		SetRefreshTokenToCookies(tokens.RefreshToken);
		return Ok(tokens);
	}

	[HttpPost]
	[Route("refresh-token")]
	public async Task<ActionResult<string>> RefreshToken(Token oldAccessToken)
	{
		if (oldAccessToken.Hash is null) return BadRequest("Invalid client request");
		var refreshToken = Request.Cookies["refresh"];
		if (refreshToken == null) return BadRequest("No refresh token was provided!");
		var tokens = await _repository.Reauthenticate(new Tokens
		{
			AccessToken = oldAccessToken,
			RefreshToken = new Token { Hash = refreshToken }
		});
		if (tokens == null) return BadRequest("Invalid access token or refresh token");
		SetRefreshTokenToCookies(tokens.RefreshToken);
		return Ok(tokens.AccessToken);
	}

	private void SetRefreshTokenToCookies(Token refreshToken)
	{
		if(refreshToken == null) return;

		var cookieOptions = new CookieOptions
		{
			HttpOnly = true,
			Expires = refreshToken.ExpiresAt
		};
		Response.Cookies.Append("refresh", refreshToken.Hash, cookieOptions);
	}

}