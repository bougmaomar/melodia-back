using AutoMapper;
using melodia.Entities;
using melodia_api.Entities;
using melodia_api.Models.SongProposals;
using melodia_api.Repositories;
using melodia_api.Repositories.Implementations;
using Microsoft.AspNetCore.Mvc;

namespace melodia_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SongProposalController : ControllerBase
{
    private readonly SongProposalRepository _repository;
    private readonly IMapper _mapper;
    private readonly IStationAccountRepository _stationAccountRepository;

    public SongProposalController(SongProposalRepository repository, IMapper mapper, IStationAccountRepository stationAccountRepository)
    {
        _repository = repository;
        _mapper = mapper;
        _stationAccountRepository = stationAccountRepository;
    }

    [HttpPost("toAccepted")]
    public async Task<IActionResult> SendToAccepted(long artistId, long songId, long radioStationId)
    {
        var result = await _repository.SendToAccepted(artistId, songId, radioStationId);

        if (!result)
        {
            return Ok(new { success = false, message = "This Song already added to accepted page." });
        }

        return Ok(new { success = true, message = "Proposal accepted successfully." });
    }
    
    
    [HttpPost("propose")]
    public async Task<IActionResult> ProposeSongs(long artistId, [FromForm] long songId, long radioStationId, string description)
    {
        var result = await _repository.ProposeSongsToRadioStation(artistId, songId, radioStationId, description);
        if (result)
        {
            return Ok("Proposals sent successfully.");
        }
        return BadRequest("Failed to send proposals.");
    }
    
    
    [HttpPost("accept")]
    public async Task<IActionResult> AcceptSongProposal(long radioStationId, long songId)
    {
        var result = await _repository.AcceptSongProposal(radioStationId, songId);
        if (result)
        {
            return Ok("Song accepted successfully.");
        }
        return BadRequest("Failed to accept song.");
    }

    
    [HttpPost("reject")]
    public async Task<IActionResult> RejectSongProposal(long radioStationId, long songId)
    {
        var result = await _repository.RejectSongProposal(radioStationId, songId);
        if (result)
        {
            return Ok("Song rejected successfully.");
        }
        return BadRequest("Failed to reject song.");
    }


    [HttpGet("all_count")]
    public async Task<IActionResult> GetAllProposalsCount()
    {
        var count = await _repository.GetAllProposalsCount();
        return Ok(count);
    }


    [HttpGet("all_accepts_count")]
    public async Task<IActionResult> GetAllAcceptedProposalsCount()
    {
        var count = await _repository.GetAllAcceptedProposalsCount();
        return Ok(count);
    }
    
    
    [HttpGet("proposals/type")]
    public async Task<IActionResult> GetProposalsByType(long stationId, long typeId)
    {
        var radioStation = await _stationAccountRepository.GetRadioStationById(stationId);
        if (radioStation == null)
        {
            return NotFound("Radion Station doesn't exist");
        }

        var proposals = await _repository.GetProposalsByTypeAndStation(stationId, typeId);

        var detailedProposals = proposals.Select(p => new
        {
            p.Id,
            p.Status,
            p.ProposalDate,
            p.ProposalDescription,
            Song = new
            {
                p.Song.Id,
                p.Song.Title,
                p.Song.ReleaseDate,
                p.Song.PlatformReleaseDate,
                p.Song.CodeISRC,
                GenreMusic = p.Song.GenreMusic?.Name,
                //Language = p.Song.Language?.Label,
                p.Song.Lyrics,
                p.Song.Duration,
                p.Song.IsMapleMusic,
                p.Song.IsMapleArtist,
                p.Song.IsMaplePerformance,
                p.Song.IsMapleLyrics,
                p.Song.YouTube,
                p.Song.Spotify,
                p.Song.Mp3FilePath,
                p.Song.WavFilePath,
                p.Song.CoverImagePath,
                Album = p.Song.Album?.Title,
                Languages =  p.Song.SongLanguages?.Select(sl => new
                {
                    sl.LanguageId,
                    sl.Language.Label,
                }).ToList(),
                Artists = p.Song.SongArtists?.Select(sa => new
                {
                    sa.ArtistId,
                    sa.Artist.Name,
                }).ToList(),
                Composers = p.Song.SongComposers?.Select(sc => new
                {
                    sc.ComposerId,
                    //sc.Composer.Name,
                }).ToList(),
                Writers = p.Song.SongWriters?.Select(sw => new
                {
                    sw.WriterId,
                    //sw.Writer.Name,
                }).ToList(),
                CrOwners = p.Song.SongCrOwners?.Select(sc => new
                {
                    sc.CROwnerId,
                    //sc.CrOwner.Name,
                }).ToList()
            }
        });

        return Ok(detailedProposals);
    }

    
    [HttpGet("proposals/language")]
    public async Task<IActionResult> GetProposalsByLanguage(long stationId, long languageId)
    {
        var radioStation = await _stationAccountRepository.GetRadioStationById(stationId);
        if (radioStation == null)
        {
            return NotFound("Radion Station doesn't exist");
        }

        var proposals = await _repository.GetProposalsBylanguageAndStation(stationId, languageId);

        var detailedProposals = proposals.Select(p => new
        {
            p.Id,
            p.Status,
            p.ProposalDate,
            p.ProposalDescription,
            Song = new
            {
                p.Song.Id,
                p.Song.Title,
                p.Song.ReleaseDate,
                p.Song.PlatformReleaseDate,
                p.Song.CodeISRC,
                GenreMusic = p.Song.GenreMusic?.Name,
                //Language = p.Song.Language?.Label,
                p.Song.Lyrics,
                p.Song.Duration,
                p.Song.IsMapleMusic,
                p.Song.IsMapleArtist,
                p.Song.IsMaplePerformance,
                p.Song.IsMapleLyrics,
                p.Song.YouTube,
                p.Song.Spotify,
                p.Song.Mp3FilePath,
                p.Song.WavFilePath,
                p.Song.CoverImagePath,
                Album = p.Song.Album?.Title,
                Languages = p.Song.SongLanguages?.Select(sl => new
                {
                    sl.LanguageId,
                    sl.Language.Label,
                }).ToList(),
                Artists = p.Song.SongArtists?.Select(sa => new
                {
                    sa.ArtistId,
                    sa.Artist.Name,
                }).ToList(),
                Composers = p.Song.SongComposers?.Select(sc => new
                {
                    sc.ComposerId,
                    //sc.Composer.Name,
                }).ToList(),
                Writers = p.Song.SongWriters?.Select(sw => new
                {
                    sw.WriterId,
                    //sw.Writer.Name,
                }).ToList(),
                CrOwners = p.Song.SongCrOwners?.Select(sc => new
                {
                    sc.CROwnerId,
                    //sc.CrOwner.Name,
                }).ToList()
            }
        });

        return Ok(detailedProposals);
    }

    
    [HttpGet("proposals/artist")]
    public async Task<IActionResult> GetProposalsByArtist(long stationId, long artistId)
    {
        var radioStation = await _stationAccountRepository.GetRadioStationById(stationId);
        if (radioStation == null)
        {
            return NotFound("Radion Station doesn't exist");
        }

        var proposals = await _repository.GetProposalsByArtistAndStation(stationId, artistId);

        var detailedProposals = proposals.Select(p => new
        {
            p.Id,
            p.Status,
            p.ProposalDate,
            p.ProposalDescription,
            Song = new
            {
                p.Song.Id,
                p.Song.Title,
                p.Song.ReleaseDate,
                p.Song.PlatformReleaseDate,
                p.Song.CodeISRC,
                GenreMusic = p.Song.GenreMusic?.Name,
                //Language = p.Song.Language?.Label,
                p.Song.Lyrics,
                p.Song.Duration,
                p.Song.IsMapleMusic,
                p.Song.IsMapleArtist,
                p.Song.IsMaplePerformance,
                p.Song.IsMapleLyrics,
                p.Song.YouTube,
                p.Song.Spotify,
                p.Song.Mp3FilePath,
                p.Song.WavFilePath,
                p.Song.CoverImagePath,
                Album = p.Song.Album?.Title,
                Languages = p.Song.SongLanguages?.Select(sl => new
                {
                    sl.Language.Id,
                    sl.Language.Label,
                }).ToList(),
                Artists = p.Song.SongArtists?.Select(sa => new
                {
                    sa.ArtistId,
                    sa.Artist.Name,
                }).ToList(),
                Composers = p.Song.SongComposers?.Select(sc => new
                {
                    sc.ComposerId,
                    //sc.Composer.Name,
                }).ToList(),
                Writers = p.Song.SongWriters?.Select(sw => new
                {
                    sw.WriterId,
                    //sw.Writer.Name,
                }).ToList(),
                CrOwners = p.Song.SongCrOwners?.Select(sc => new
                {
                    sc.CROwnerId,
                    //sc.CrOwner.Name,
                }).ToList()
            }
        });

        return Ok(detailedProposals);
    }

    
    [HttpGet("proposals")]
    public async Task<IActionResult> GetProposalsByRadioStation(long radioStationId, ProposalStatus? status = null)
{
    var radioStation = await _stationAccountRepository.GetRadioStationById(radioStationId);
    if (radioStation == null)
    {
        return NotFound("Radio station not found.");
    }

    var proposals = await _repository.GetProposalsByRadioStationId(radioStationId, status);

    var detailedProposals = proposals.Select(p => new
    {
        p.Id,
        p.Status,
        p.ProposalDate,
        p.ProposalDescription,
        Song = new
        {
            p.Song.Id,
            p.Song.Title,
            p.Song.ReleaseDate,
            p.Song.PlatformReleaseDate,
            p.Song.CodeISRC,
            GenreMusic = p.Song.GenreMusic?.Name,
            //Language = p.Song.Language?.Label,
            p.Song.Lyrics,
            p.Song.Duration,
            p.Song.IsMapleMusic,
            p.Song.IsMapleArtist,
            p.Song.IsMaplePerformance,
            p.Song.IsMapleLyrics,
            p.Song.YouTube,
            p.Song.Spotify,
            p.Song.Mp3FilePath,
            p.Song.WavFilePath,
            p.Song.CoverImagePath,
            Album = p.Song.Album?.Title,
            //Languages = p.Song.SongLanguages?.ToList(),
            Languages = p.Song.SongLanguages?.Select(sl => new
            {
                sl.Language.Id,
                sl.Language.Label,
            }).ToList(),
            Artists = p.Song.SongArtists?.Select(sa => new
            {
                //sa.Artist.Id,
                sa.Artist.Name,
                //sa.Artist.PhotoProfile,
                //sa.Artist.FirstName,
                //sa.Artist.LastName,
                //sa.Artist.Bio,
                //sa.Artist.Google,
                //sa.Artist.Facebook,
                //sa.Artist.Instagram,
                //sa.Artist.Youtube,
                //sa.Artist.Spotify,
                //sa.Artist.CareerStartDate,
                //sa.Artist.Active,
                //Agent = sa.Artist.Agent != null ? new
                //{
                //    sa.Artist.Agent.Id,
                //    sa.Artist.Agent.FirstName,
                //    sa.Artist.Agent.LastName
                //} : null,
                //City = sa.Artist.City != null ? new
                //{
                //    sa.Artist.City.Id,
                //    sa.Artist.City.Name
                //} : null,
                //Account = sa.Artist.Account != null ? new
                //{
                //    sa.Artist.Account.Id,
                //    sa.Artist.Account.Email,
                //    sa.Artist.Account.UserName
                //} : null
            }).ToList(),
            Composers = p.Song.SongComposers?.Select(sc => new
            {
                sc.ComposerId
                //sc.Composer.Name
            }).ToList(),
            Writers = p.Song.SongWriters?.Select(sw => new
            {
                sw.WriterId
                //sw.Writer.Name
            }).ToList(),
            CrOwners = p.Song.SongCrOwners?.Select(sc => new
            {
                sc.CROwnerId
                //sc.CrOwner.Name
            }).ToList()
        }
    });

    return Ok(detailedProposals);
}


    [HttpGet("accepted_proposals")]
    public async Task<IActionResult> GetAcceptedProposalsByRadioStation(long radioStationId)
    {
        var radioStation = await _stationAccountRepository.GetRadioStationById(radioStationId);
        if (radioStation == null)
        {
            return NotFound("Radio station not found.");
        }

        var proposals = await _repository.GetAcceptedProposalsByRadioStation(radioStationId);

        var detailedProposals = proposals.Select(p => new
        {
            p.Id,
            p.Status,
            p.ProposalDate,
            Song = new
            {
                p.Song.Id,
                p.Song.Title,
                p.Song.ReleaseDate,
                p.Song.PlatformReleaseDate,
                p.Song.CodeISRC,
                GenreMusic = p.Song.GenreMusic?.Name,
                p.Song.Lyrics,
                p.Song.Duration,
                p.Song.IsMapleMusic,
                p.Song.IsMapleArtist,
                p.Song.IsMaplePerformance,
                p.Song.IsMapleLyrics,
                p.Song.YouTube,
                p.Song.Spotify,
                p.Song.Mp3FilePath,
                p.Song.WavFilePath,
                p.Song.CoverImagePath,
                Album = p.Song.Album?.Title,
                Languages = p.Song.SongLanguages?.Select(sl => new
                {
                    sl.Language.Id,
                    sl.Language.Label,
                }).ToList(),
                Artists = p.Song.SongArtists?.Select(sa => new
                {
                    Name = sa.Artist.FirstName + " " + sa.Artist.LastName,
                }).ToList(),
                Composers = p.Song.SongComposers?.Select(sc => new
                {
                    sc.ComposerId,
                    //sc.Composer.Name
                }).ToList(),
                Writers = p.Song.SongWriters?.Select(sw => new
                {
                    sw.WriterId,
                    //sw.Writer.Name
                }).ToList(),
                CrOwners = p.Song.SongCrOwners?.Select(sc => new
                {
                    sc.CROwnerId,
                    //sc.CrOwner.Name
                }).ToList()
            }
        });

        return Ok(detailedProposals);
    }


    [HttpGet("all_proposals_by_artists")]
    public async Task<IActionResult> GetAllProposalsByArtist(long artistId)
    {
        var proposals = await _repository.GetAllProposalsByArtist(artistId);
        var detailedProposals = proposals.Select(p => new
        {
            p.Id,
            p.Status,
            p.ProposalDate,
            p.Artist.Name,
            p.RadioStation.StationName,
            Song = new
            {
                p.Song.Id,
                p.Song.Title,
                p.Song.ReleaseDate,
                p.Song.PlatformReleaseDate,
                p.Song.CodeISRC,
                GenreMusic = p.Song.GenreMusic?.Name,
                //Language = p.Song.Language?.Label,
                p.Song.Lyrics,
                p.Song.Duration,
                p.Song.IsMapleMusic,
                p.Song.IsMapleArtist,
                p.Song.IsMaplePerformance,
                p.Song.IsMapleLyrics,
                p.Song.YouTube,
                p.Song.Spotify,
                p.Song.Mp3FilePath,
                p.Song.WavFilePath,
                p.Song.CoverImagePath,
                Album = p.Song.Album?.Title,
                Languages = p.Song.SongLanguages?.Select(sl => new
                {
                    sl.LanguageId,
                    //sl.Language.Label,
                }).ToList(),
                Artists = p.Song.SongArtists?.Select(sa => new
                {
                    sa.Artist.Name,
                }).ToList(),
                Composers = p.Song.SongComposers?.Select(sc => new
                {
                    sc.ComposerId,
                    //sc.Composer.Name
                }).ToList(),
                Writers = p.Song.SongWriters?.Select(sw => new
                {
                    sw.WriterId,
                    //sw.Writer.Name
                }).ToList(),
                CrOwners = p.Song.SongCrOwners?.Select(sc => new
                {
                    sc.CROwnerId,
                    //sc.CrOwner.Name
                }).ToList()
            }
        });

        return Ok(detailedProposals);
    }
    
    
    [HttpGet("all_accepted_proposals_artist")]
    public async Task<IActionResult> GetAllAcceptedProposalsByArtist(long artistId)
    {
        var proposals = await _repository.GetAllAcceptedProposalsByArtist(artistId);

        var detailedProposals = proposals.Select(p => new
        {
            p.Id,
            p.Status,
            p.ProposalDate,
            p.Artist.Name,
            p.RadioStation.StationName,
            Song = new
            {
                p.Song.Id,
                p.Song.Title,
                p.Song.ReleaseDate,
                p.Song.PlatformReleaseDate,
                p.Song.CodeISRC,
                GenreMusic = p.Song.GenreMusic?.Name,
                //Language = p.Song.Language?.Label,
                p.Song.Lyrics,
                p.Song.Duration,
                p.Song.IsMapleMusic,
                p.Song.IsMapleArtist,
                p.Song.IsMaplePerformance,
                p.Song.IsMapleLyrics,
                p.Song.YouTube,
                p.Song.Spotify,
                p.Song.Mp3FilePath,
                p.Song.WavFilePath,
                p.Song.CoverImagePath,
                Album = p.Song.Album?.Title,
                Languages = p.Song.SongLanguages?.Select(sl => new
                {
                    sl.LanguageId,
                    //sl.Language.Label,
                }).ToList(),
                Artists = p.Song.SongArtists?.Select(sa => new
                {
                    sa.Artist.Name,
                }).ToList(),
                Composers = p.Song.SongComposers?.Select(sc => new
                {
                    sc.ComposerId,
                    //sc.Composer.Name
                }).ToList(),
                Writers = p.Song.SongWriters?.Select(sw => new
                {
                    sw.WriterId,
                    //sw.Writer.Name
                }).ToList(),
                CrOwners = p.Song.SongCrOwners?.Select(sc => new
                {
                    sc.CROwnerId,
                    //sc.CrOwner.Name
                }).ToList()
            }
        });

        return Ok(detailedProposals);
    }

    
    [HttpGet("all_accepted_proposals_by_agent")]
    public async Task<IActionResult> GetAllAcceptedProposalsByAgent(long agentId)
    {
        var proposals = await _repository.GetAllAcceptedProposalsByAgent(agentId);
        var ProposalsView = _mapper.Map<List<SongProposalsView>>(proposals);
        return Ok(ProposalsView);
    }
    
    
    [HttpGet("mensual_proposals")]
    public async Task<IActionResult> GetMensualProposalsByArtist(long artistId)
    {
        var mensuals = await _repository.GetMensualProposalsByArtist(artistId);
        return Ok(mensuals);
    }

    
    [HttpGet("annual_proposals")]
    public async Task<IActionResult> GetAnnualProposalsByArtist(long artistId)
    {
        var annuals = await _repository.GetAnnualProposalsByArtist(artistId);
        return Ok(annuals);
    }

    [HttpGet("proposals_stats")]
    public async Task<IActionResult> GetProposalsStatistics()
    {
        var proposals = await _repository.GetProposalsStatistics();
        return Ok(proposals);
    }
}