using melodia.Configurations;
using melodia.Entities;
using melodia_api.Models.AgentAccount;
using melodia_api.Models.Song;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace melodia_api.Repositories.Implementations
{
    public class AgentAccountRepository : IAgentAccountRepository
    {
        private readonly MelodiaDbContext _db;
        private readonly ILogger<AgentAccountRepository> _logger;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<Account> _userManager;
        public readonly string _fileDirectory;

        public AgentAccountRepository(UserManager<Account> userManager, RoleManager<Role> roleManager,
            MelodiaDbContext db, IConfiguration configuration, ILogger<AgentAccountRepository> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
            _fileDirectory = configuration["FileStorage:Directory"];
            _logger = logger;
        }

        public async Task<List<AgentAccountViewDto>> GetAllAgentAccounts()
        {
            var agents = await _db.Agents
                .Include(a => a.Account)
                .Where(a => a.Active)
                .ToListAsync();

            var dtos = new List<AgentAccountViewDto>();

            foreach (var a in agents)
            {
                if (a.Account != null)
                {
                    dtos.Add(new AgentAccountViewDto
                    {
                        Id = a.Account.Id,
                        Email = a.Account.Email,
                        PhoneNumber = a.Account.PhoneNumber,
                        CareerStartDate = a.CareerStartDate,
                        PhotoProfile = a.PhotoProfile,
                        Status = a.Status,
                        ArtistsNum = a.ArtistsNum,
                        WebSite = a.WebSite,
                        AgentId = a.Id,
                        AgentRealName = $"{a.FirstName} {a.LastName}",
                        LastLogin = a.Account.LastLogin,
                        Active = a.Account.Active
                    });
                }
            }

            return dtos;
        }

        public async Task<List<Song>> GetAllSongByAgent(long agentId)
        {
            var artistIds = await _db.Artists
                .Where(a => a.Active == true && a.AgentId == agentId)
                .Select(a => a.Id)
                .ToListAsync();

            var songs = await _db.Songs
                .Include(s => s.SongArtists)
                .ThenInclude(sa => sa.Artist)
                .ThenInclude(saa => saa.Account)
                .Include(s => s.GenreMusic)
                .Include(s => s.Album)
                .Where(s => s.SongArtists.Any(sa => artistIds.Contains(sa.ArtistId)) && s.Active == true )
                .Distinct()
                .ToListAsync();

            return songs;
        }

        public async Task<List<Album>> GetAllAlbumsByAgent(long agentId)
        {
            var artistIds = await _db.Artists
                .Where(a => a.Active == true && a.AgentId == agentId)
                .Select(a => a.Id)
                .ToListAsync();

            var albums = await _db.Albums
                .Include(s => s.AlbumArtists)
                .ThenInclude(sa => sa.Artist)
                .Include(s => s.AlbumType)
                .Include(s => s.Songs)
                .Where(s => s.AlbumArtists.Any(sa => artistIds.Contains(sa.ArtistId)))
                .Distinct()
                .ToListAsync();

            return albums;
        }



        public async Task<Agent> CreateAgentAndAccount(AgentAccountCreateDto accountCreateDto)
        {
            try
            {
                Agent agent = null;
                var strategy = _db.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = _db.Database.BeginTransaction())
                    {
                        var existingAccount = await _db.Accounts.FirstOrDefaultAsync(a => a.Email == accountCreateDto.Email);
                        if (existingAccount != null) throw new Exception("Un compte avec cet email existe déjà.");
        
                        agent = new Agent
                        {
                            FirstName = accountCreateDto.FirstName,
                            LastName = accountCreateDto.LastName,
                            CareerStartDate = accountCreateDto.CareerStartDate,
                            PhoneNumber = accountCreateDto.PhoneNumber,
                            WebSite = accountCreateDto.WebSite,
                            Status = "ToAccept",
                            Active = true
                        };
        
                        _db.Agents.Add(agent);
                        await _db.SaveChangesAsync();
        
                        var account = new Account
                        {
                            Id = Guid.NewGuid().ToString(), // ADD THIS LINE
                            UserName = accountCreateDto.UserName,
                            Email = accountCreateDto.Email,
                            PhoneNumber = accountCreateDto.PhoneNumber,
                            Agent = agent,
                            RefreshToken = "refresh_token",
                            Active = true
                        };
        
                        var accountCreateResult = await _userManager.CreateAsync(account, accountCreateDto.Password);
                        if (!accountCreateResult.Succeeded)
                            throw new Exception(string.Join(", ", accountCreateResult.Errors.Select(x => x.Description)));
        
                        var role = await _db.Roles.FirstOrDefaultAsync(r => r.Name == "Agent");
                        if (role == null)
                        {
                            role = new Role { Name = "Agent", NormalizedName = "AGENT", Active = true };
                            var roleCreateResult = await _roleManager.CreateAsync(role);
                            if (!roleCreateResult.Succeeded)
                            {
                                throw new Exception(string.Join(", ", roleCreateResult.Errors.Select(x => x.Description)));
                            }
                        }
        
                        await _userManager.AddToRoleAsync(account, "Agent");
                        await _db.SaveChangesAsync();
                        transaction.Commit();
                    }
                });
                return agent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the agent account");
                throw;
            }
        }
        public async Task<Agent> AcceptAgentAccount(long id)
        {
            var agent = await _db.Agents
                .Include(a => a.Account)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (agent == null) throw new Exception("Agent not found.");

            agent.Status = "Accepted";
            await _db.SaveChangesAsync();

            return agent;
        }


        public async Task<Agent> RejectAgentAccount(long id)
        {
            var agent = await _db.Agents
                .Include(a => a.Account)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (agent == null) throw new Exception("Agent not found.");

            agent.Status = "Rejected";
            await _db.SaveChangesAsync();

            return agent;
        }

        public async Task<AgentAccountViewDto> UpdateAgentAndAccountAsync(AgentAccountUpdateDto agentAccountUpdateDto)
        {
            var account = await _db.Users.FindAsync(agentAccountUpdateDto.accountId);

            if (account == null) throw new Exception("Account not found.");

            var agent = await _db.Agents.FindAsync(agentAccountUpdateDto.AgentId);

            if (agent == null) throw new Exception("Agent not found.");

            account.Email = agentAccountUpdateDto.Email;
            account.PhoneNumber = agentAccountUpdateDto.PhoneNumber;

            if (agentAccountUpdateDto.CityId != -1) agent.CityId = agentAccountUpdateDto.CityId;
            agent.FirstName = agentAccountUpdateDto.FirstName;
            agent.LastName = agentAccountUpdateDto.LastName;
            agent.Bio = agentAccountUpdateDto.Bio;
            agent.WebSite = agentAccountUpdateDto.WebSite == "null" ? null : agentAccountUpdateDto.WebSite;
            agent.CareerStartDate = agentAccountUpdateDto.CareerStartDate;
            if (agentAccountUpdateDto.PhotoProfile != null)
            {
                if (!string.IsNullOrEmpty(agent.PhotoProfile))
                {
                    DeleteFile(agent.PhotoProfile);
                }
                agent.PhotoProfile = await UploadFileAsync(agentAccountUpdateDto.PhotoProfile, "images");
            }
            await _db.SaveChangesAsync();

            var agentAccountViewDto = new AgentAccountViewDto
            {
                Id = agent.Account.Id,
                AgentRealName = agent.FirstName + " " + agent.LastName,
                CareerStartDate = agent.CareerStartDate,
                Email = account.Email,
                PhoneNumber = account.PhoneNumber,
                Password = agent.Account.PasswordHash,
                AgentId = agent.Id,
                CityId = agent.CityId,
                Active = true
            };

            return agentAccountViewDto;
        }

        //public async Task<> GetAgentPartnersByEmail(string email)
        //{

        //}
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

        //public async Task<Account> GetAgentAccountById(string accountId)
        //{
        //    var account = await _userManager.FindByIdAsync(accountId);
        //    return account;
        //}
        public async Task<AgentAccountViewDto> GetAgentAccountById(long id)
        {
            var agent =  await _db.Agents
                .Include(a => a.Account)
                .FirstOrDefaultAsync(a => a.Id == id);

            return new AgentAccountViewDto
            {
                Id = agent.Account.Id,
                Email = agent.Account.Email,
                PhoneNumber = agent.Account.PhoneNumber,
                ArtistsNum = agent.ArtistsNum,
                PhotoProfile = agent.PhotoProfile,
                AgentId = agent.Id,
                AgentRealName = $"{agent.FirstName} {agent.LastName}",
                CareerStartDate = agent.CareerStartDate,
                CityId = agent.CityId,
                LastLogin = agent.Account.LastLogin,
                WebSite = agent.WebSite,
                Status = agent.Status,
                Active = agent.Account.Active
            };
        }

        public async Task<AgentAccountViewDto> GetAgentAccountByEmail(string email)
        {
            var agent = await _db.Agents
                .Include(a => a.Account)
                .FirstOrDefaultAsync(a => a.Account.Email == email && a.Active);

            
                return new AgentAccountViewDto
                {
                    Id = agent.Account.Id,
                    Email = agent.Account.Email,
                    PhoneNumber = agent.Account.PhoneNumber,
                    PhotoProfile = agent.PhotoProfile,
                    AgentId = agent.Id,
                    AgentRealName = $"{agent.FirstName} {agent.LastName}",
                    CareerStartDate = agent.CareerStartDate,
                    Bio = agent.Bio,
                    ArtistsNum = agent.ArtistsNum,
                    CityId = agent.CityId,
                    LastLogin = agent.Account.LastLogin,
                    Active = agent.Account.Active,
                    Status = agent.Status,
                    WebSite = agent.WebSite
                };
            
        }


        public async Task<bool> DeactivateAgentAccountById(string accountId)
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

        public async Task<bool> ActivateAgentAccountById(string accountId)
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
    }
}
