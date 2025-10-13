using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using melodia_api.Exceptions;
using melodia_api.Models.ArtistAccount;
using melodia.Configurations;
using melodia.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using melodia_api.Models.Account;
using DocumentFormat.OpenXml.Drawing.Charts;
using melodia_api.Entities;
using melodia_api.Models.AgentAccount;
using melodia_api.Models.Album;

namespace melodia_api.Repositories.Implementations;

public class ArtistAccountRepository : IArtistAccountRepository
{
    private readonly MelodiaDbContext _db;
    private readonly RoleManager<Role> _roleManager;
    private readonly UserManager<Account> _userManager;
    public readonly string _fileDirectory;

    public ArtistAccountRepository(UserManager<Account> userManager, RoleManager<Role> roleManager,
        MelodiaDbContext db, IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _db = db;
        _fileDirectory = configuration["FileStorage:Directory"];
    }

    public async Task<List<ArtistAccountViewDto>> GetAllArtistAccounts()
    {
        var artists = await _db.Artists
            .Include(a => a.Account)
            .Where(a => a.Active)
            .ToListAsync();

        var dtos = new List<ArtistAccountViewDto>();

        foreach (var a in artists)
        {
            if (a.Account != null)
            {
                dtos.Add(new ArtistAccountViewDto
                {
                    Id = a.Account.Id,
                    Email = a.Account.Email,
                    PhoneNumber = a.Account.PhoneNumber,
                    PhotoProfile = a.PhotoProfile,
                    CareerStartDate = a.CareerStartDate,
                    ArtistId = a.Id,
                    ArtistRealName = $"{a.FirstName} {a.LastName}",
                    LastLogin = a.Account.LastLogin,
                    AgentId = a.AgentId,
                    Active = a.Account.Active
                });
            }
        }

        return dtos;
    }

    public async Task<List<ArtistAccountViewDto>> GetArtistsByAgent(long id)
    {
        var artists = await _db.Artists
           .Include(a => a.Account)
           .Where(a => a.Active && a.AgentId == id)

           .ToListAsync();

        var dtos = new List<ArtistAccountViewDto>();

        foreach (var a in artists)
        {
            if (a.Account != null)
            {
                dtos.Add(new ArtistAccountViewDto
                {
                    Id = a.Account.Id,
                    Email = a.Account.Email,
                    PhoneNumber = a.Account.PhoneNumber,
                    PhotoProfile = a.PhotoProfile,
                    CareerStartDate = a.CareerStartDate,
                    ArtistId = a.Id,
                    ArtistRealName = $"{a.FirstName} {a.LastName}",
                    Name = $"{a.Name}",
                    LastLogin = a.Account.LastLogin,
                    Active = a.Account.Active
                });
            }
        }

        return dtos;
    }

    public async Task<Artist> CreateArtistAndAccount(ArtistAccountCreateDto accountCreateDto)
{
    Artist artist = null;

    var strategy = _db.Database.CreateExecutionStrategy();

    await strategy.ExecuteAsync(async () =>
    {
        using (var transaction = _db.Database.BeginTransaction())
        {
            var existingAccount = await _db.Accounts.FirstOrDefaultAsync(a => a.Email == accountCreateDto.Email);
            if (existingAccount != null) throw new Exception("Un compte avec cet email existe déjà.");

            artist = new Artist
            {
                Name = accountCreateDto.Name,
                FirstName = accountCreateDto.FirstName,
                LastName = accountCreateDto.LastName,
                CareerStartDate = accountCreateDto.CareerStartDate,
                Active = true,
            };
            
            _db.Artists.Add(artist);
            await _db.SaveChangesAsync();

            var account = new Account
            {
                UserName = NormalizeUserName(accountCreateDto.Name),
                Email = accountCreateDto.Email,
                PhoneNumber = accountCreateDto.PhoneNumber,
                Artist = artist,
                RefreshToken = "refresh_token",
                Active = true
            };

            var accountCreateResult = await _userManager.CreateAsync(account, accountCreateDto.Password);
            if (!accountCreateResult.Succeeded) throw new Exception(string.Join(", ", accountCreateResult.Errors.Select(x => x.Description)));

            var role = await _db.Roles.FirstOrDefaultAsync(r => r.Name == "Artist");
            if (role == null)
            {
                role = new Role { Name = "Artist", NormalizedName = "ARTIST", Active = true };
                var roleCreateResult = await _roleManager.CreateAsync(role);
                if (!roleCreateResult.Succeeded)
                {
                    throw new Exception(string.Join(", ", roleCreateResult.Errors.Select(x => x.Description)));
                }
            }

            await _userManager.AddToRoleAsync(account, "Artist");

            await _db.SaveChangesAsync();
            transaction.Commit();
        }
    });

    return artist;
}
    private static string NormalizeUserName(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return "";

        var normalized = input.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in normalized)
        {
            var uc = CharUnicodeInfo.GetUnicodeCategory(c);
            if (uc != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        return Regex.Replace(sb.ToString(), @"[^a-zA-Z0-9]", "");
    }

    public async Task<Artist> CreateArtistByAgent(ArtistCreateByAgentDto artistCreateByAgentDto)
    {
        Artist artist = null;

        var strategy = _db.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                var existingAccount = await _db.Accounts.FirstOrDefaultAsync(a => a.Email == artistCreateByAgentDto.Email);
                if (existingAccount != null) throw new Exception("Un compte avec cet email existe déjà.");

                artist = new Artist
                {
                    Name = artistCreateByAgentDto.Name,
                    FirstName = artistCreateByAgentDto.FirstName,
                    LastName = artistCreateByAgentDto.LastName,
                    CareerStartDate = artistCreateByAgentDto.CareerStartDate,
                    Bio = artistCreateByAgentDto.Bio,
                    AgentId = artistCreateByAgentDto.AgentId,
                    //CityId = artistCreateByAgentDto.CityId,
                    Facebook = artistCreateByAgentDto.Facebook,
                    Youtube = artistCreateByAgentDto.Youtube,
                    Spotify = artistCreateByAgentDto.Spotify,
                    Instagram = artistCreateByAgentDto.Instagram,
                    Google = artistCreateByAgentDto.Google,
                    Active = true,
                };

                _db.Artists.Add(artist);
                await _db.SaveChangesAsync();

                var account = new Account
                {
                    UserName = artistCreateByAgentDto.Name,
                    Email = artistCreateByAgentDto.Email,
                    PhoneNumber = artistCreateByAgentDto.PhoneNumber,
                    Artist = artist,
                    RefreshToken = "refresh_token",
                    Active = true
                };
                var artistPassword = "Artist."+ artistCreateByAgentDto.FirstName+ "123@";
                var accountCreateResult = await _userManager.CreateAsync(account, artistPassword);
                if (!accountCreateResult.Succeeded) throw new Exception(string.Join(", ", accountCreateResult.Errors.Select(x => x.Description)));


                var role = await _db.Roles.FirstOrDefaultAsync(r => r.Name == "Artist");
                if (role == null)
                {
                    role = new Role { Name = "Artist", NormalizedName = "ARTIST", Active = true };
                    var roleCreateResult = await _roleManager.CreateAsync(role);
                    if (!roleCreateResult.Succeeded)
                    {
                        throw new Exception(string.Join(", ", roleCreateResult.Errors.Select(x => x.Description)));
                    }
                }

                
                await _userManager.AddToRoleAsync(account, "Artist");

                await _db.SaveChangesAsync();

                if (artistCreateByAgentDto.AgentId != null)
                {
                        var agentArtist = new ArtistAgent
                        {
                            AgentId = artistCreateByAgentDto.AgentId,
                            ArtistId = artist.Id,

                        };
                        _db.ArtistAgent.Add(agentArtist);
                        await _db.SaveChangesAsync();
                }
                transaction.Commit();
            }
        });

        return artist;
    } 
    
    public async Task<Artist> UpdateArtistByAgent(ArtistUpdateByAgentDto artistUpdateByAgentDto)
{
    var strategy = _db.Database.CreateExecutionStrategy();

    return await strategy.ExecuteAsync(async () =>
    {
        using (var transaction = await _db.Database.BeginTransactionAsync())
        {
            var artist = await _db.Artists.Include(a => a.Account)
                                          .FirstOrDefaultAsync(a => a.Id == artistUpdateByAgentDto.Artistid);
            if (artist == null)
            {
                throw new Exception("Artist not found.");
            }

            // Update artist properties
            artist.Name = artistUpdateByAgentDto.Name;
            artist.FirstName = artistUpdateByAgentDto.FirstName;
            artist.LastName = artistUpdateByAgentDto.LastName;
            artist.CareerStartDate = artistUpdateByAgentDto.CareerStartDate;
            artist.Bio = artistUpdateByAgentDto.Bio;
            artist.AgentId = artistUpdateByAgentDto.AgentId;
            artist.Facebook = artistUpdateByAgentDto.Facebook == "null" ? null : artistUpdateByAgentDto.Facebook;
            artist.Youtube = artistUpdateByAgentDto.Youtube == "null" ? null : artistUpdateByAgentDto.Youtube;
            artist.Spotify = artistUpdateByAgentDto.Spotify == "null" ? null : artistUpdateByAgentDto.Spotify;
            artist.Instagram = artistUpdateByAgentDto.Instagram == "null" ? null : artistUpdateByAgentDto.Instagram;
            artist.Google = artistUpdateByAgentDto.Google == "null" ? null : artistUpdateByAgentDto.Google;
            artist.Active = artistUpdateByAgentDto.Active;

            // Handle PhotoProfile
            if (artistUpdateByAgentDto.PhotoProfile != null)
            {
                if (!string.IsNullOrEmpty(artist.PhotoProfile))
                {
                    DeleteFile(artist.PhotoProfile); // Method to delete existing file
                }
                artist.PhotoProfile = await UploadFileAsync(artistUpdateByAgentDto.PhotoProfile, "images"); // Method to upload new file
            }

            // Update associated account details
            var account = artist.Account;
            if (account != null)
            {
                account.UserName = artistUpdateByAgentDto.Name;
                account.Email = artistUpdateByAgentDto.Email;
                account.PhoneNumber = artistUpdateByAgentDto.PhoneNumber;
            }

            await _db.SaveChangesAsync();

            await transaction.CommitAsync();

            return artist;
        }
    });
}
        
    public async Task<ArtistAccountViewDto> UpdateArtistAndAccountAsync(ArtistAccountUpdateDto artistAccountUpdateDto)
    {
        var account = await _db.Users.FindAsync(artistAccountUpdateDto.accountId);

        if (account == null) throw new Exception("Account not found.");

        var artist = await _db.Artists.FindAsync(artistAccountUpdateDto.ArtistId);

        if (artist == null) throw new Exception("Artist not found.");

        account.Email = artistAccountUpdateDto.Email;
        account.PhoneNumber = artistAccountUpdateDto.PhoneNumber;
        artist.Bio = artistAccountUpdateDto.Bio;
        artist.Google = artistAccountUpdateDto.Google;
        artist.Facebook = artistAccountUpdateDto.Facebook;
        artist.Youtube = artistAccountUpdateDto.Youtube;
        artist.Spotify = artistAccountUpdateDto.Spotify;
        artist.Instagram = artistAccountUpdateDto.Instagram;
        artist.Name = artistAccountUpdateDto.Name;
        artist.AgentId = artistAccountUpdateDto.AgentId;
        if(artistAccountUpdateDto.CityId != -1) artist.CityId = artistAccountUpdateDto.CityId;
        artist.FirstName = artistAccountUpdateDto.FirstName;
        artist.LastName = artistAccountUpdateDto.LastName;
        artist.CareerStartDate = artistAccountUpdateDto.CareerStartDate;
        if (artistAccountUpdateDto.PhotoProfile != null)
        {
            if (!string.IsNullOrEmpty(artist.PhotoProfile))
            {
                DeleteFile(artist.PhotoProfile);
            }
            artist.PhotoProfile = await UploadFileAsync(artistAccountUpdateDto.PhotoProfile, "images");
        }
        await _db.SaveChangesAsync();

        var artistAccountViewDto = new ArtistAccountViewDto
        {
            Id = artist.Account.Id,
            Name = artist.Name,
            ArtistRealName = artist.FirstName + " " + artist.LastName,
            CareerStartDate = artist.CareerStartDate,
            Email = account.Email,
            Bio = artist.Bio,
            Google = artist.Google,
            Youtube = artist.Youtube,
            Spotify = artist.Spotify,
            Instagram = artist.Instagram,
            Facebook = artist.Facebook,
            PhoneNumber = account.PhoneNumber,
            Password = artist.Account.PasswordHash,
            ArtistId = artist.Id,
            CityId = artist.CityId,
            AgentId = artist.AgentId,
            Active = true
        };

        return artistAccountViewDto;
    }
    
    public async Task<IdentityResult> ChangePassword(string email, string oldPassword, string newPassword)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return IdentityResult.Failed(new IdentityError { Description = "Account not found." });
        
        var passwordVerificationResult = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, oldPassword);
        if (passwordVerificationResult != PasswordVerificationResult.Success)
        {
            return IdentityResult.Failed(new IdentityError { Description = "Incorrect old password." });
        }
        
        if (passwordVerificationResult == PasswordVerificationResult.Success &&
            _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, newPassword) == PasswordVerificationResult.Success)
        {
            return IdentityResult.Failed(new IdentityError { Description = "New password cannot be the same as the old password." });
        }
        
        var passwordValidationResult = await _userManager.PasswordValidators[0].ValidateAsync(_userManager, user, newPassword);
        if (!passwordValidationResult.Succeeded)
        {
            return passwordValidationResult;
        }
        
        var hashedNewPassword = _userManager.PasswordHasher.HashPassword(user, newPassword);
        user.PasswordHash = hashedNewPassword;
        var updateResult = await _userManager.UpdateAsync(user);
        return updateResult;
    }

    public async Task<Account> GetArtistAccountById(string accountId)
    {
        var account = await _userManager.FindByIdAsync(accountId);
        return account;
    }
    
    public async Task<ArtistAccountViewDto> GetArtistAccountById(long id)
    {
        var artist = await _db.Artists
            .Include(a => a.Account)
            .FirstOrDefaultAsync(a => a.Id == id);

        return new ArtistAccountViewDto
        {
            Id = artist.Account.Id,
            Email = artist.Account.Email,
            PhoneNumber = artist.Account.PhoneNumber,
            PhotoProfile = artist.PhotoProfile,
            CareerStartDate = artist.CareerStartDate,
            CityId = artist.CityId,
            LastLogin = artist.Account.LastLogin,
            Active = artist.Account.Active,
            Name = artist.Name,
            ArtistRealName = $"{artist.FirstName} {artist.LastName}",
            Bio = artist.Bio,
            Google = artist.Google,
            Facebook = artist.Facebook,
            Instagram = artist.Instagram,
            Youtube = artist.Youtube,
            Spotify = artist.Spotify,
            ArtistId = artist.Id,
            //NumberOfAlbums = artist.Albums.Count + artist.AlbumArtists.Select(aa => aa.Album).Distinct().Count(),
            //NumberOfSingles = artist.SongArtists.Where(sa => sa.Active && sa.Song.AlbumId == null).Select(sa => sa.Song).Distinct().Count()
        };
    }

    public async Task<ArtistAccountViewDto> GetArtistAccountByEmail(string email) 
    { 
        var artist = await _db.Artists
            .Include(a => a.Account)
            .Include(a => a.SongArtists).ThenInclude(sa => sa.Song)
            .Include(a => a.Albums).ThenInclude(album => album.Songs)
            .Include(a => a.AlbumArtists).ThenInclude(aa => aa.Album).ThenInclude(album => album.Songs)
            .FirstOrDefaultAsync(a => a.Account.Email == email && a.Active);
        
        if (artist != null && artist.Account != null) 
        { 
            var albumSongs = new HashSet<long>();
            
            foreach (var album in artist.Albums) { 
                foreach (var song in album.Songs) { 
                    albumSongs.Add(song.Id); 
                } 
            }
            
            foreach (var albumArtist in artist.AlbumArtists) { 
                foreach (var song in albumArtist.Album.Songs) { 
                    albumSongs.Add(song.Id); 
                } 
            }
            
            var singlesCount = artist.SongArtists.Where(sa => sa.Active && sa.Song.AlbumId == null).Select(sa => sa.Song).Distinct().Count();
            
            var albumCount = artist.Albums.Count + artist.AlbumArtists.Select(aa => aa.Album).Distinct().Count();
            
            return new ArtistAccountViewDto { 
                Id = artist.Account.Id, 
                Email = artist.Account.Email, 
                PhoneNumber = artist.Account.PhoneNumber, 
                PhotoProfile = artist.PhotoProfile,
                Name = artist.Name,
                Bio = artist.Bio,
                Google = artist.Google,
                Facebook = artist.Facebook,
                Youtube = artist.Youtube,
                Instagram = artist.Instagram,
                Spotify = artist.Spotify,
                ArtistId = artist.Id, 
                ArtistRealName = $"{artist.FirstName} {artist.LastName}", 
                CareerStartDate = artist.CareerStartDate, 
                CityId = artist.CityId, 
                LastLogin = artist.Account.LastLogin, 
                Active = artist.Account.Active, 
                NumberOfAlbums = albumCount, 
                NumberOfSingles = singlesCount, 
                NumberOfAlbumSongs = albumSongs.Count 
            }; 
        }
        else { 
            return null; 
        } 
    }

    public async Task<bool> DeactivateArtistAccountById(string accountId)
    {
        var account = await _userManager.FindByIdAsync(accountId);
        if (account != null)
        {
            account.Active = false;
            var result = await _userManager.UpdateAsync(account);
            return result.Succeeded;
        }

        return false;
    }

    public async Task<bool> ActivateArtistAccountById(string accountId)
    {
        var account = await _userManager.FindByIdAsync(accountId);
        if (account != null)
        {
            account.Active = true;
            var result = await _userManager.UpdateAsync(account);
            return result.Succeeded;
        }

        return false;
    }
    
    private async Task<string> UploadFileAsync(IFormFile file, string subdirectory)
    {
        var uploadDirectory = Path.Combine(_fileDirectory, subdirectory);

        if (!Directory.Exists(uploadDirectory))
        {
            Directory.CreateDirectory(uploadDirectory);
        }

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadDirectory, fileName);

        var relativeFilePath = Path.GetRelativePath(_fileDirectory, filePath).Replace("\\", "/");

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return relativeFilePath;
    }
    
    private void DeleteFile(string filePath)
    {
        var fullPath = Path.Combine(_fileDirectory, filePath);
        if (File.Exists(fullPath)) File.Delete(fullPath);
    }
    
    public async Task<Artist> GetArtistById(long id)
    {
        return await _db.Artists.Include(a => a.Albums)
            .ThenInclude(al => al.Songs)
            .FirstOrDefaultAsync(a => a.Id == id);
    }
    
    public async Task<bool> SendProposal(Artist artist, Song song, RadioStation radioStation, string description)
    {
       
            var proposal = new Proposal
            {
                ArtistId = artist.Id,
                SongId = song.Id,
                RadioStationId = radioStation.Id,
                Status = ProposalStatus.Pending,
                ProposalDescription = description
            };

            await _db.Proposals.AddAsync(proposal);
    

        return await _db.SaveChangesAsync() > 0;
    }

    public async Task RecordVisitAsync(long artistId, long radioStationId)
    {
        var visit = new Visit
        {
            ArtistId = artistId,
            RadioStationId = radioStationId
        };

        _db.Visits.Add(visit);
        await _db.SaveChangesAsync();
    }

    public async Task<List<Visit>> GetAllVisitsByArtists(long artistId)
    {
        return await _db.Visits
            .Include(v => v.Artist)
            .Include(v => v.RadioStation)
            .Where(v => v.ArtistId == artistId)
            .ToListAsync();
    }

    public static string GenerateRandomString(int length)
    {
        const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        const string numbers = "0123456789";
        const string specialCharacters = "@#$%^&*()!";

        Random random = new Random();

        char letter = letters[random.Next(letters.Length)];
        char number = numbers[random.Next(numbers.Length)];
        char specialCharacter = specialCharacters[random.Next(specialCharacters.Length)];

        string remainingCharacters = new string(Enumerable.Repeat(letters + numbers + specialCharacters, length - 3)
                                    .Select(s => s[random.Next(s.Length)]).ToArray());

        string combinedPassword = letter.ToString() + number.ToString() + specialCharacter.ToString() + remainingCharacters;

        string shuffledPassword = new string(combinedPassword.OrderBy(c => random.Next()).ToArray());

        return shuffledPassword;
    }
    
    public async Task<int[]> GetMensualVisitsByArtist(long artistId)
    {
        var currentYear = DateTime.Now.Year;

        int[] monthlyVisits = new int[12];

        var visits = _db.Visits
            .Where(v => v.ArtistId == artistId && v.VisitDate.Year == currentYear)
            .ToList();

        foreach (var visit in visits)
        {
            int monthIndex = visit.VisitDate.Month - 1;
            monthlyVisits[monthIndex]++;
        }

        return monthlyVisits;
    }

    public async Task<int[]> GetAnnualVisitsByArtist(long artistId)
    {
        int[] annualVisits = new int[7];
        int currentYear = DateTime.Now.Year;

        var visits = _db.Visits
            .Where(p => p.ArtistId == artistId && p.VisitDate.Year >= currentYear - 6)
            .ToList();

        foreach (var visit in visits)
        {
            int year = visit.VisitDate.Year;
            int yearIndex = currentYear - year;

            if (yearIndex >= 0 && yearIndex < 7)
            {
                annualVisits[yearIndex]++;
            }
        }

        return annualVisits.Reverse().ToArray();
    }

    public async Task<List<ArtistPlayComparaison>> GetArtistComparison(long artistId, long comparedId)
    {
        // Get the last 12 months dynamically
        var last12Months = Enumerable.Range(0, 12)
            .Select(i => DateTime.UtcNow.AddMonths(-i))
            .OrderBy(d => d)
            .Select(d => new { Year = d.Year, Month = d.Month })
            .ToList();

        // Get the minimum date for filtering
        var minDate = new DateTime(last12Months.Min(m => m.Year), last12Months.Min(m => m.Month), 1);

        // Fetch play counts from the database
        var plays = await _db.Plays
            .Where(p => p.Song.SongArtists.Any(s => s.ArtistId == artistId) || 
                        p.Song.SongArtists.Any(s => s.ArtistId == comparedId))
            .Where(p => p.PlayDate >= minDate) // Move DateTime creation out of LINQ-to-Entities
            .Select(p => new
            {
                Year = p.PlayDate.Year,
                Month = p.PlayDate.Month,
                ArtistId = p.Song.SongArtists.First().ArtistId
            })
            .ToListAsync(); // Fetch into memory

        // Group and count plays after fetching
        var groupedPlays = plays
            .GroupBy(p => new { p.Year, p.Month, p.ArtistId })
            .Select(g => new
            {
                g.Key.ArtistId,
                g.Key.Year,
                g.Key.Month,
                PlayCount = g.Count()
            })
            .ToList();

        // Map results to the expected format
        var result = last12Months.Select(m => new ArtistPlayComparaison
        {
            Month = new DateTime(m.Year, m.Month, 1).ToString("MMM"), // Safe in-memory operation
            You = groupedPlays.FirstOrDefault(p => p.ArtistId == artistId && p.Year == m.Year && p.Month == m.Month)?.PlayCount ?? 0,
            Comparison = groupedPlays.FirstOrDefault(p => p.ArtistId == comparedId && p.Year == m.Year && p.Month == m.Month)?.PlayCount ?? 0
        }).ToList();

        return result;
    }

    

}

