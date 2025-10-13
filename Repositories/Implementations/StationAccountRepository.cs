using DocumentFormat.OpenXml.Bibliography;
using melodia_api.Entities;
using melodia_api.Models.Employee;
using melodia.Configurations;
using melodia.Entities;
using melodia_api.Models.StationAccount;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace melodia_api.Repositories.Implementations
{
    public class StationAccountRepository : IStationAccountRepository
    {
        private readonly MelodiaDbContext _db;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<Account> _userManager;
        public readonly string _fileDirectory;

        public StationAccountRepository(UserManager<Account> userManager, RoleManager<Role> roleManager,
            MelodiaDbContext db, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
            _fileDirectory = configuration["FileStorage:Directory"];
        } 
        public async Task<List<StationAccountViewDto>> GetAllStationAccounts() 
        { 
            var stations = await _db.Stations
                .Include(a => a.Account)
                .Include(a => a.City)
                .ThenInclude(c => c.Country)
                .Include(a => a.StationType)
                .Include(a => a.StationLanguages)
                .ThenInclude(sl => sl.Language)
                .Include(a => a.StationMusicFormats)
                .ThenInclude(sm => sm.MusicFormat)
                .Include(a => a.Programmes)
                .Include(a => a.Employees)
                .ThenInclude(e => e.Position)
                .Where(a => a.Active)
                .ToListAsync();
            
            var dtos = new List<StationAccountViewDto>();
            
            foreach (var a in stations) 
            { 
                if (a.Account != null) 
                { 
                    var employeeDtos = a.Employees.Select(e => new EmployeeViewDto 
                    { 
                        Id = e.Id, 
                        FirstName = e.FirstName, 
                        LastName = e.LastName, 
                        Sexe = e.Sexe, 
                        HiringDate = e.HiringDate, 
                        DepartureDate = e.DepartureDate, 
                        PositionName = e.Position.Name, 
                        Active = e.Active 
                    }).ToList();
                    
                    dtos.Add(new StationAccountViewDto 
                    { 
                        Id = a.Account.Id, 
                        Email = a.Account.Email, 
                        PhoneNumber = a.Account.PhoneNumber, 
                        FoundationDate = a.FoundationDate, 
                        Logo = a.Logo, 
                        StationId = a.Id, 
                        StationName = a.StationName, 
                        LastLogin = a.Account.LastLogin, 
                        Active = a.Account.Active, 
                        Frequency = a.Frequency,
                        Status = a.Status,
                        WebSite = a.WebSite, 
                        StationOwner = a.StationOwner, 
                        CityId = a.CityId, 
                        StationTypeName = a.StationType.Name, 
                        StationLanguages = a.StationLanguages.Select(sl => sl.Language.Label).ToList(), 
                        StationMusicFormats = a.StationMusicFormats.Select(sm => sm.MusicFormat.Name).ToList(), 
                        ProgramNames = a.Programmes.Select(p => p.Title).ToList(), 
                        Employees = employeeDtos 
                    }); 
                } 
            }
            
            return dtos; 
        }
        
        public async Task<RadioStation> CreateStationAndAccount(StationAccountCreateDto accountCreateDto)
        {
            RadioStation station = null;

            var strategy = _db.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _db.Database.BeginTransaction())
                {
                    var existingAccount = await _db.Accounts.FirstOrDefaultAsync(a => a.Email == accountCreateDto.Email);
                    if (existingAccount != null) throw new Exception("Un compte avec cet email existe déjà.");

                    var station = new RadioStation
                    {
                        StationName = accountCreateDto.StationName,
                        FoundationDate = accountCreateDto.FoundationDate,
                        StationOwner = accountCreateDto.StationOwner,
                        Frequency = accountCreateDto.Frequency,
                        StationTypeId = accountCreateDto.StationTypeId,
                        PhoneNumber = accountCreateDto.PhoneNumber,
                        Status = "ToAccept",
                        Active = true
                    };
                    
                    _db.Stations.Add(station);
                    await _db.SaveChangesAsync();

                    var account = new Account
                    {
                        UserName = accountCreateDto.StationName,
                        Email = accountCreateDto.Email,
                        PhoneNumber = accountCreateDto.PhoneNumber,
                        RadioStation = station,
                        RefreshToken = "refresh_token",
                        Active = true
                    };

                    var accountCreateResult = await _userManager.CreateAsync(account, accountCreateDto.Password);
                    if (!accountCreateResult.Succeeded) throw new Exception(string.Join(", ", accountCreateResult.Errors.Select(x => x.Description)));

                    var role = await _db.Roles.FirstOrDefaultAsync(r => r.Name == "Station");
                    if (role == null)
                    {
                        role = new Role { Name = "Station", NormalizedName = "STATION", Active = true };
                        var roleCreateResult = await _roleManager.CreateAsync(role);
                        if (!roleCreateResult.Succeeded)
                        {
                            throw new Exception(string.Join(", ", roleCreateResult.Errors.Select(x => x.Description)));
                        }
                    }

                    await _userManager.AddToRoleAsync(account, "Station");

                    await _db.SaveChangesAsync();
                    transaction.Commit();
                }
            });

            return station;
        }
        
        public async Task<RadioStation> AcceptStationAccount(long id)
        {
            var station = await _db.Stations
                .Include(a => a.Account)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (station == null) throw new Exception("Station not found.");

            station.Status = "Accepted";
            await _db.SaveChangesAsync();

            return station;
        }
        
        public async Task<RadioStation> RejectStationAccount(long id)
        {
            var station = await _db.Stations
                .Include(a => a.Account)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (station == null) throw new Exception("Agent not found.");

            station.Status = "Rejected";
            await _db.SaveChangesAsync();

            return station;
        }

        public async Task<StationAccountViewDto> UpdateStationAndAccountAsync(StationAccountUpdateDto stationAccountUpdateDto)
        {
            var account = await _db.Users.FindAsync(stationAccountUpdateDto.accountId);

            if (account == null) throw new Exception("Account not found.");

            var station = await _db.Stations.FindAsync(stationAccountUpdateDto.StationId);

            if (station == null) throw new Exception("Station not found.");

            account.Email = stationAccountUpdateDto.Email;
            station.PhoneNumber = stationAccountUpdateDto.PhoneNumber;
            station.Description = stationAccountUpdateDto.Description;
            station.Frequency = stationAccountUpdateDto.Frequency;
            station.WebSite = stationAccountUpdateDto.WebSite;
            station.StationName = stationAccountUpdateDto.StationName;
            station.StationOwner = stationAccountUpdateDto.StationOwner;
            station.StationTypeId = stationAccountUpdateDto.StationTypeId;
            if (stationAccountUpdateDto.CityId != -1) station.CityId = stationAccountUpdateDto.CityId;
            station.FoundationDate = stationAccountUpdateDto.FoundationDate;
            if (stationAccountUpdateDto.Logo != null)
            {
                if (!string.IsNullOrEmpty(station.Logo))
                {
                    DeleteFile(station.Logo);
                }
                station.Logo = await UploadFileAsync(stationAccountUpdateDto.Logo, "images");
            }
            await _db.SaveChangesAsync();

            var stationAccountViewDto = new StationAccountViewDto
            {
                Id = station.Account.Id,
                StationName = station.StationName,
                FoundationDate = station.FoundationDate,
                Description = station.Description,
                Email = account.Email,
                PhoneNumber = station.PhoneNumber,
                Password = station.Account.PasswordHash,
                StationId = station.Id,
                WebSite = station.WebSite,
                CityId = station.CityId,
                Active = true
            };

            return stationAccountViewDto;
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

        public async Task<Account> GetStationAccountById(string accountId)
        {
            var account = await _userManager.FindByIdAsync(accountId);
            return account;
        }

        public async Task<string> GetTopStation()
        {
            var topStation = await _db.Proposals
                .GroupBy(p => p.RadioStationId)
                .Select(g => new 
                { 
                    StationId = g.Key, 
                    ProposalCount = g.Count() 
                })
                .OrderByDescending(g => g.ProposalCount)
                .FirstOrDefaultAsync();

            if (topStation == null)
                return "No station found";

            var station = await _db.Stations
                .Where(s => s.Id == topStation.StationId)
                .Select(s => s.StationName)
                .FirstOrDefaultAsync();

            return station ?? "Station";
        }

        public async Task<StationAccountViewDto> GetStationAccountByEmail(string email)
        {
            var station = await _db.Stations
                .Include(a => a.Account)
                .Include(a => a.StationType)
                .Include(a => a.Programmes)
                .Include(a => a.Employees)
                .FirstOrDefaultAsync(a => a.Account.Email == email && a.Active);

            if (station != null && station.Account != null)
            {
                return new StationAccountViewDto
                {
                    Id = station.Account.Id,
                    Email = station.Account.Email,
                    PhoneNumber = station.PhoneNumber,
                    Description = station.Description,
                    Logo = station.Logo,
                    StationId = station.Id,
                    StationName = station.StationName,
                    FoundationDate = station.FoundationDate,
                    Frequency = station.Frequency,
                    WebSite = station.WebSite,
                    StationOwner = station.StationOwner,
                    StationTypeName = station.StationType.Name,
                    StationTypeId = station.StationType.Id,
                    StationLanguages = station.StationLanguages?.Select(l => l.Language.Label).ToList() ?? new List<string>(),
                    StationMusicFormats = station.StationMusicFormats?.Select(m => m.MusicFormat.Name).ToList() ?? new List<string>(),
                    CityId = station.CityId,
                    LastLogin = station.Account.LastLogin,
                    Active = station.Account.Active,
                    Status = station.Status
                };
            }
            else
            {
                return null;
            }
        }
        
        public async Task<bool> DeactivateStationAccountById(string accountId)
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

        public async Task<bool> ActivateStationAccountById(string accountId)
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
        
        public async Task<RadioStation> GetRadioStationById(long id)
        {
            return await _db.Stations
                .Include(rs => rs.Programmes)
                .Include(rs => rs.StationLanguages)
                .Include(rs => rs.StationMusicFormats)
                .Include(rs => rs.Account)
                .Include(rs => rs.City)
                .Include(rs => rs.StationType)
                .Include(rs => rs.Employees)
                .Include(rs => rs.Proposals)
                .FirstOrDefaultAsync(rs => rs.Id == id);
        }

        public async Task<bool> SendToAccepted(melodia.Entities.Artist artist, Song song, RadioStation radioStation)
        {
            var existingProposal = await _db.Proposals
                .FirstOrDefaultAsync(p => p.ArtistId == artist.Id && p.SongId == song.Id && p.RadioStationId == radioStation.Id && p.Status == ProposalStatus.Accepted);

            if (existingProposal != null)
            {
                return false;
            }
            else
            {
                var proposal = new Proposal
                {
                    ArtistId = artist.Id,
                    SongId = song.Id,
                    RadioStationId = radioStation.Id,
                    Status = ProposalStatus.Accepted
                };

                await _db.Proposals.AddAsync(proposal);

                return await _db.SaveChangesAsync() > 0;
            }
        }
        
        public async Task<bool> AcceptSong(long radioStationId, long songId)
        {
            var proposals = await _db.Proposals.Where(p => p.RadioStationId == radioStationId && p.SongId == songId).ToListAsync();
            if (proposals == null) return false;
            foreach (var proposal in proposals)
            {
                if (proposal.Status == ProposalStatus.Pending)
                {
                    proposal.Status = ProposalStatus.Accepted;
                    _db.Proposals.Update(proposal);
                }
            }
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> RejectSong(long radioStationId, long songId)
        {
            var proposals = await _db.Proposals.Where(p => p.RadioStationId == radioStationId && p.SongId == songId).ToListAsync();
            if (proposals == null) return false;

            foreach (var proposal in proposals)
            {
                if (proposal.Status == ProposalStatus.Pending)
                {
                    proposal.Status = ProposalStatus.Rejected;
                    _db.Proposals.Update(proposal);
                }
                
            }
            return await _db.SaveChangesAsync() > 0;
        }
        
        public async Task<List<Proposal>> GetProposalsByRadioStationIdAsync(long radioStationId, ProposalStatus? status = null)
        {
            var query = _db.Proposals.AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(p => p.RadioStationId == radioStationId && p.Status == status.Value);
            }
            else
            {
                query = query.Where(p => p.RadioStationId == radioStationId);
            }

            var proposals = await query
                //.Include(p => p.)
                .Include(p => p.Song)
                .ThenInclude(s => s.SongLanguages)
                .ThenInclude(sl => sl.Language)
                .Include(p => p.Song)
                .ThenInclude(s => s.Album)
                .Include(p => p.Song)
                .ThenInclude(s => s.GenreMusic)
                .Include(p => p.Song)
                .ThenInclude(s => s.SongArtists)
                .Include(p => p.Song)
                .ThenInclude(s => s.SongCrOwners)
                .Include(p => p.Song)
                .ThenInclude(s => s.SongPOwners)
                .Include(p => p.Song)
                .ThenInclude(s => s.SongComposers)
                .Include(p => p.Artist).ThenInclude(s => s.Account)
                //.Where(p => p.Status == ProposalStatus.Pending)
                .ToListAsync();

            return proposals;
        }
        
        public async Task<List<Proposal>> GetAllAcceptedProposalsByArtist(long artistId)
        {
            var proposals = _db.Proposals.AsQueryable();
            proposals = proposals.Where(p => p.ArtistId == artistId && p.Status == ProposalStatus.Accepted);
            return await proposals
                .Include(p => p.Song)
                .ThenInclude(s => s.SongLanguages)
                .ThenInclude(sl => sl.Language)
                .Include(p => p.Song)
                .ThenInclude(s => s.Album)
                .Include(p => p.Song)
                .ThenInclude(s => s.GenreMusic)
                .Include(s => s.Song)
                .ThenInclude(s => s.SongArtists)
                .Include(p => p.Song)
                .ThenInclude(s => s.SongCrOwners)
                .Include(p => p.Song)
                .ThenInclude(s => s.SongPOwners)
                .Include(p => p.Song)
                .ThenInclude(s => s.SongComposers)
                .Include(p => p.Artist)
                .ThenInclude(s => s.Account)
                .Include(p => p.RadioStation)
                .ThenInclude(s => s.Account)
                .ToListAsync();
        }

        public async Task<List<Proposal>> GetAllAcceptedProposalsByAgent(long agentId)
        {
            var artists = await _db.Artists.Where(a => a.AgentId == agentId).ToListAsync();
            var proposals = new List<Proposal>();
            foreach (var artist in artists)
            {
                var proposalsAr = await GetAllAcceptedProposalsByArtist(artist.Id);
                proposals.AddRange(proposalsAr);
            }
            return proposals;
        }
        
        public async Task<List<Proposal>> GetAcceptedProposalsByRadioStation(long radioStationId)
        {
            var query = _db.Proposals.AsQueryable();

            query = query.Where(p => p.RadioStationId == radioStationId && p.Status == ProposalStatus.Accepted);
            
            var proposals = await query
                .Include(p => p.Song)
                .ThenInclude(s => s.SongLanguages)
                .ThenInclude(sl => sl.Language)
                .Include(p => p.Song)
                .ThenInclude(s => s.Album)
                .Include(p => p.Song)
                .ThenInclude(s => s.GenreMusic)
                .Include(s => s.Song)
                .ThenInclude(s => s.SongArtists)
                .Include(p => p.Song)
                .ThenInclude(s => s.SongCrOwners)
                .Include(p => p.Song)
                .ThenInclude(s => s.SongPOwners)
                .Include(p => p.Song)
                .ThenInclude(s => s.SongComposers)
                .Include(p => p.Artist).ThenInclude(s => s.Account)
                .ToListAsync();

            return proposals;
        }

        public async Task<bool> RadioStationExists(long radioStationId)
        {
            return await _db.Stations.AnyAsync(rs => rs.Id == radioStationId);
        }
        
        public async Task<RadioStation> GetRadioStationByAccountId(string accountId)
        {
            return await _db.Stations
                .Include(rs => rs.Account)
                .Include(rs => rs.City)
                .Include(rs => rs.StationType)
                .Include(rs => rs.StationLanguages)
                .Include(rs => rs.StationMusicFormats)
                .Include(rs => rs.Programmes)
                .Include(rs => rs.Employees)
                .Include(rs => rs.Proposals)
                .FirstOrDefaultAsync(rs => rs.Account.Id == accountId && rs.Active);
        }

    }
}
