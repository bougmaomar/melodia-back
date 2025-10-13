using melodia_api.Entities;
using melodia.Entities;
using NAudio.MediaFoundation;
using melodia.Configurations;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.InkML;

namespace melodia_api.Repositories.Implementations;

public class SongProposalRepository 
{
    private readonly MelodiaDbContext _db;
    private readonly IArtistAccountRepository _artistRepository;
    private readonly ISongRepository _songRepository;
    private readonly IStationAccountRepository _radioStationRepository;
    
    public SongProposalRepository(MelodiaDbContext db, IArtistAccountRepository artistRepository, ISongRepository songRepository, IStationAccountRepository radioStationRepository)
    {
        _db = db;
        _artistRepository = artistRepository;
        _songRepository = songRepository;
        _radioStationRepository = radioStationRepository;
    }
    
    public async Task<bool> ProposeSongsToRadioStation(long artistId, long songId, long radioStationId, string description)
    {
        var artist = await _artistRepository.GetArtistById(artistId);
        var radioStation = await _radioStationRepository.GetRadioStationById(radioStationId);
        
            var song = await _songRepository.GetSongById(songId);
            if (song == null)
            {
                return false;
            }        

        if (artist == null || radioStation == null)
        {
            return false;
        }

        return await _artistRepository.SendProposal(artist, song, radioStation, description);
    }

   public async Task<bool> SendToAccepted(long artistId, long songId, long radioStationId)
    {
        var artist = await _artistRepository.GetArtistById(artistId);
        var radioStation = await _radioStationRepository.GetRadioStationById(radioStationId);

        var song = await _songRepository.GetSongById(songId);
        if (song == null)
        {
            return false;
        }

        if (artist == null || radioStation == null)
        {
            return false;
        }

        return await _radioStationRepository.SendToAccepted(artist, song, radioStation);
    }

    public async Task<bool> AcceptSongProposal(long radioStationId, long songId)
    {
        var radioStation = await _radioStationRepository.GetRadioStationById(radioStationId);
        var song = await _songRepository.GetSongById(songId);

        if (radioStation == null || song == null)
        {
            return false;
        }

        return await _radioStationRepository.AcceptSong(radioStationId, songId);
    }

    public async Task<bool> RejectSongProposal(long radioStationId, long songId)
    {
        var radioStation = await _radioStationRepository.GetRadioStationById(radioStationId);
        var song = await _songRepository.GetSongById(songId);

        if (radioStation == null || song == null)
        {
            return false;
        }

        return await _radioStationRepository.RejectSong(radioStationId, songId);
    }

    public async Task<List<Proposal>> GetAllAcceptedProposalsByArtist(long artistId)
    {
        return await _radioStationRepository.GetAllAcceptedProposalsByArtist(artistId);
    }

    public async Task<List<Proposal>> GetAllAcceptedProposalsByAgent(long agentId)
    {
        return await _radioStationRepository.GetAllAcceptedProposalsByAgent(agentId);
    }
    
    public async Task<List<Proposal>> GetAllProposalsByArtist(long artistId)
    {
        var proposals = _db.Proposals.AsQueryable();
        proposals = proposals.Where(p => p.ArtistId == artistId );
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

    public async Task<int> GetAllProposalsCount()
    {
        var count = await _db.Proposals.CountAsync();
        return count;
    }

    public async Task<int> GetAllAcceptedProposalsCount()
    {
        var count = await _db.Proposals.Where(p => p.Status == ProposalStatus.Accepted).CountAsync();
        return count;
    }
    
    public async Task<List<Proposal>> GetProposalsByRadioStationId(long radioStationId, ProposalStatus? status = null)
    {
        return await _radioStationRepository.GetProposalsByRadioStationIdAsync(radioStationId, status);
    }

    public async Task<List<Proposal>> GetProposalsByTypeAndStation(long stationId, long typeId)
    {
        return await _db.Proposals
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
                .Where(p =>p.RadioStationId == stationId && p.Status == ProposalStatus.Pending && p.Song.GenreMusic.Id == typeId)
                .ToListAsync();
    }

    public async Task<List<Proposal>> GetProposalsBylanguageAndStation(long stationId, long languageId)
    {
        return await _db.Proposals
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
                .Where(p => p.RadioStationId == stationId && p.Status == ProposalStatus.Pending && p.Song.SongLanguages.Any(sl => sl.LanguageId == languageId))
                .ToListAsync();
    }

    public async Task<List<Proposal>> GetProposalsByArtistAndStation(long stationId, long artistId)
    {
        return await _db.Proposals
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
                .Where(p => p.RadioStationId == stationId && p.Status == ProposalStatus.Pending && p.Song.SongArtists.Any(aa => aa.ArtistId == artistId))
                .ToListAsync();
    }
    
    public async Task<List<Proposal>> GetAcceptedProposalsByRadioStation(long radioStationId)
    {
        return await _radioStationRepository.GetAcceptedProposalsByRadioStation(radioStationId);
    }

    public async Task<int[]> GetMensualProposalsByArtist(long artistId)
    {
        var currentYear = DateTime.Now.Year;

        int[] monthlyProposals = new int[12];

        var proposals = _db.Proposals
            .Where(p => p.ArtistId == artistId && p.ProposalDate.Year == currentYear)
            .ToList();

        foreach (var proposal in proposals)
        {
            int monthIndex = proposal.ProposalDate.Month - 1; 
            monthlyProposals[monthIndex]++;
        }

        return monthlyProposals;
    }

    public async Task<int[]> GetAnnualProposalsByArtist(long artistId)
    {
        int[] annualProposals = new int[7];
        int currentYear = DateTime.Now.Year;

        var proposals = _db.Proposals
            .Where(p => p.ArtistId == artistId && p.ProposalDate.Year >= currentYear - 6)
            .ToList();

        foreach (var proposal in proposals)
        {
            int year = proposal.ProposalDate.Year;
            int yearIndex = currentYear - year;

            if (yearIndex >= 0 && yearIndex < 7)
            {
                annualProposals[yearIndex]++;
            }
        }

        return annualProposals.Reverse().ToArray();
    }

    public async Task<int> GetProposalsStatistics()
    {
        var currentDate = DateTime.UtcNow; 
        var firstDayOfCurrentMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
        var firstDayOfLastMonth = firstDayOfCurrentMonth.AddMonths(-1);
        var lastDayOfLastMonth = firstDayOfCurrentMonth.AddDays(-1);

        var currentMonthCount = await _db.Proposals
            .Where(p => p.ProposalDate >= firstDayOfCurrentMonth)
            .CountAsync();

        var lastMonthCount = await _db.Proposals
            .Where(p => p.ProposalDate >= firstDayOfLastMonth && p.ProposalDate <= lastDayOfLastMonth)
            .CountAsync();

        return currentMonthCount - lastMonthCount;
        
    }
}