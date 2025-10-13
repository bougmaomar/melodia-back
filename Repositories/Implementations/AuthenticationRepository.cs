using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using melodia_api.Models;
using melodia.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using melodia_api.Models.Account;

namespace melodia_api.Repositories.Implementations;

public class AuthenticationRepository : IAuthenticationRepository
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<Account> _userManager;
    
    public AuthenticationRepository(IConfiguration configuration, UserManager<Account> userManager)
    {
        _userManager = userManager;
        _configuration = configuration;
    }
    
    public async Task<Tokens> Authenticate(string email, string password)
{
    // Retrieve the account with its roles and related entities
    var account = await _userManager.Users
        .Include(a => a.AccountRoles.Where(ar => ar.Role.Active))
        .ThenInclude(ar => ar.Role)
        .Include(a => a.Agent)
        .Include(a => a.Artist)
        .Include(a => a.RadioStation)
        .FirstOrDefaultAsync(a => a.Email == email && a.Active);

    if (account == null) return null;

    // Check if the account has the "Agent" role and the agent's status
    var agentRole = account.AccountRoles.FirstOrDefault(ar => ar.Role.Name == "Agent");
    if (agentRole != null)
    {
        var agentStatus = account.Agent.Status;
        if (agentStatus == "ToAccept" || agentStatus == "Rejected")
        {
            return new Tokens { Status = agentStatus };
        }
    }
    
    var stationRole = account.AccountRoles.FirstOrDefault(ar => ar.Role.Name == "Station");
    if (stationRole != null)
    {
        var stationStatus = account.RadioStation.Status;
        if (stationStatus == "ToAccept" || stationStatus == "Rejected")
        {
            return new Tokens { Status = stationStatus };
        }
    }

    // Validate the password
    var isPasswordValid = await _userManager.CheckPasswordAsync(account, password);
    if (!isPasswordValid) return null;

    // Initialize user info
    var user = new UserInfo
    {
        Email = account.Email,
        Id = account.Id,
        Name = account.UserName,
        Role = account.AccountRoles.FirstOrDefault()?.Role.Name,
        Photo = GetUserPhoto(account),
        UserId = GetUserId(account)
    };

    // Generate tokens
    var accessToken = CreateToken(account);
    var refreshToken = GenerateRefreshToken();

    // Update account with refresh token and login time
    account.RefreshToken = refreshToken.Hash;
    account.RefreshTokenExpiryTime = refreshToken.ExpiresAt;
    account.LastLogin = DateTime.Now;
    await _userManager.UpdateAsync(account);

    // Return the tokens
    return new Tokens
    {
        AccessToken = accessToken,
        RefreshToken = refreshToken,
        User = user
    };
}

// Helper method to get user photo based on role
private string GetUserPhoto(Account account)
{
    return account.AccountRoles.FirstOrDefault()?.Role.Name switch
    {
        "Artist" => account.Artist.PhotoProfile,
        "Agent" => account.Agent.PhotoProfile,
        "Station" => account.RadioStation.Logo,
        _ => null
    };
}

// Helper method to get user ID based on role
private long GetUserId(Account account)
{
    return account.AccountRoles.FirstOrDefault()?.Role.Name switch
    {
        "Artist" => account.Artist.Id,
        "Agent" => account.Agent.Id,
        "Station" => account.RadioStation.Id,
        _ => 0
    };
}

    
    public async Task<Tokens> Reauthenticate(Tokens tokens)
    {
        var accessToken = tokens.AccessToken;
        var refreshToken = tokens.RefreshToken;
        var principal = GetPrincipalFromExpiredToken(accessToken);
        if (principal == null) return null;
        var username = principal.Identity?.Name;
        var account = await _userManager.Users
            .Include(a => a.AccountRoles.Where(ar => ar.Role.Active))
            .ThenInclude(ar => ar.Role)
            .Include(a => a.Agent)
            .Include(a => a.Artist)
            .Include(a => a.RadioStation)
            .FirstOrDefaultAsync(a => (a.UserName == username) & a.Active);
        // If the refresh token is expired, the user has to authenticate a normal authentication
        if (account == null || account.RefreshToken != refreshToken.Hash || account.RefreshTokenExpiryTime <= DateTime.Now) return null;
        var newAccessToken = CreateToken(principal.Claims.ToList());
        account.RefreshToken = refreshToken.Hash;
        refreshToken.ExpiresAt = account.RefreshTokenExpiryTime;
        account.LastLogin = DateTime.Now;
        await _userManager.UpdateAsync(account);
        return new Tokens { AccessToken = newAccessToken, RefreshToken = refreshToken };
    }
    private Token CreateToken(Account account)
    {
        _ = int.TryParse(_configuration["jwt:TokenValidityInMinutes"], out var tokenValidityInMinutes);
        var expiration = DateTime.UtcNow.AddMinutes(tokenValidityInMinutes);
        var token = CreateJwtToken(
            CreateClaims(account),
            CreateSigningCredentials(),
            expiration
        );
        var tokenHandler = new JwtSecurityTokenHandler();
        return new Token
        {
            Hash = tokenHandler.WriteToken(token),
            ExpiresAt = expiration
        };
    }
    
     private Token CreateToken(IEnumerable<Claim> claims)
    {
        _ = int.TryParse(_configuration["jwt:TokenValidityInMinutes"], out var tokenValidityInMinutes);
        var expiration = DateTime.UtcNow.AddMinutes(tokenValidityInMinutes);
        var token = CreateJwtToken(
            claims,
            CreateSigningCredentials(),
            expiration
        );
        var tokenHandler = new JwtSecurityTokenHandler();
        return new Token
        {
            Hash = tokenHandler.WriteToken(token),
            ExpiresAt = expiration
        };
    }
     
    private Token GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out var refreshTokenValidityInDays);
        return new Token
        {
            Hash = Convert.ToBase64String(randomNumber),
            ExpiresAt = DateTime.Now.AddDays(refreshTokenValidityInDays)
        };
    }
    
    private JwtSecurityToken CreateJwtToken(IEnumerable<Claim> claims, SigningCredentials credentials,
        DateTime expiration)
    {
        return new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: expiration,
            signingCredentials: credentials
        );
    }
    
    private IEnumerable<Claim> CreateClaims(Account account)
    {
        var roleClaims = new List<Claim>();

        if(account?.AccountRoles != null) 
        {
            foreach (var role in account.AccountRoles)
            {
                if(role?.Role != null)
                {
                    roleClaims.Add(new Claim(ClaimTypes.Role, role.Role.Name ?? string.Empty)); 
                    roleClaims.Add(new Claim("RoleId", role.Role.Id.ToString() ?? string.Empty)); 
                }
            }
        }
    
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"] ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
            new Claim("AccountId", account?.Id ?? string.Empty),
            new Claim(ClaimTypes.Name, account?.UserName ?? string.Empty),
            new Claim(ClaimTypes.Email, account?.Email ?? string.Empty)
        };

        return claims.Concat(roleClaims);
    }

    private SigningCredentials CreateSigningCredentials()
    {
        return new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
            ),
            SecurityAlgorithms.HmacSha256
        );
    }
    private ClaimsPrincipal GetPrincipalFromExpiredToken(Token accessToken)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:key"])),
            ValidateLifetime = false
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(accessToken.Hash, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");
        return principal;
    }
}