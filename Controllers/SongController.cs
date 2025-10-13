using AutoMapper;
using melodia_api.Models.Song;
using melodia_api.Repositories;
using melodia.Entities;
using Microsoft.AspNetCore.Mvc;
using melodia_api.Entities;

namespace melodia_api.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class SongController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ISongRepository _repository;

    public SongController(IMapper mapper, ISongRepository repository)
    {
        _mapper = mapper;
        _repository = repository;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SongViewDto>>> GetAllSongs()
    {
        var songs = await _repository.GetAllSongs();
        return Ok(_mapper.Map<IEnumerable<SongViewDto>>(songs));
    }

    [HttpGet("system")]
    public async Task<ActionResult<IEnumerable<SongViewDto>>> GetSongsForStationn()
    {
        var songs = await _repository.GetSongsForStation();
        return Ok(_mapper.Map<IEnumerable<SongViewDto>>(songs));
    }

    [HttpGet("song_plays/{songId}")]
    public async Task<ActionResult<int>> GetSongPlays(long songId)
    {
        return await _repository.GetSongPlays(songId);
    }

    [HttpGet("song_downloads/{songId}")]
    public async Task<ActionResult<int>> GetSongDownloads(long songId)
    {
        return await _repository.GetSongDownloads(songId);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<SongViewDto>> GetSongById(long id)
    {
        var song = await _repository.FindSongById(id);

        if (song == null) return NotFound();
        
        return Ok(_mapper.Map<SongViewDto>(song));
    }

    [HttpGet("song/{id}")]
    public async Task<ActionResult<SongView>> GetOneSong(long id)
    {
        var song = await _repository.GetSongById(id);

        if (song == null) return NotFound();

        return Ok(_mapper.Map<SongView>(song));
    }

    [HttpGet("relatedsongs/{id}")]
    public async Task<ActionResult<IEnumerable<SongViewDto>>> GetRelatedSongs(long id)
    {
        var songs = await _repository.GetRelatedSongs(id);
        return Ok(_mapper.Map<IEnumerable<SongViewDto>>(songs));
    }

    [HttpGet("artist/{email}")]
    public async Task<ActionResult<SongViewDto>> GetSongByArtistEmail(string email)
    {
        var songs = await _repository.FindSongByArtistEmail(email);

        if (songs == null) return NotFound();
        
        return Ok(_mapper.Map<IEnumerable<SongViewDto>>(songs));
    }

    [HttpGet("language/{languageId}")]
    public async Task<ActionResult<SongViewDto>> FindSongsByLanguage(long languageId)
    {
        var songs = await _repository.FindSongsByLanguage(languageId);

        if (songs == null) return NotFound();

        return Ok(_mapper.Map<IEnumerable<SongViewDto>>(songs));
    }

    [HttpGet("genre/{typeId}")]
    public async Task<ActionResult<SongViewDto>> FindSongsByType(long typeId)
    {
        var songs = await _repository.FindSongsByType(typeId);
        if (songs == null) return NotFound();
        return Ok(_mapper.Map<IEnumerable<SongViewDto>>(songs));
    }

    [HttpGet("artistId/{artistId}")]
    public async Task<ActionResult<SongViewDto>> FindSongsByArtist(long artistId)
    {
        var songs = await _repository.FindSongsByArtist(artistId);
        if (songs == null) return NotFound();
        return Ok(_mapper.Map<IEnumerable<SongViewDto>>(songs));
    }
    
    [HttpGet("account/{userId}")]
    public async Task<ActionResult<IEnumerable<SongViewDto>>> GetFavoriteSongs(string userId)
    {
        var albums = await _repository.GetFavoriteSongs(userId);
        return Ok(_mapper.Map<IEnumerable<SongViewDto>>(albums));
    }
    
    [HttpPost]
    public async Task<ActionResult<Song>> CreateSong([FromForm] SongCreateDto songCreateDto)
    {
        Console.WriteLine("Message de log simple");

        try
        {
            var song = await _repository.CreateSong(songCreateDto);
            var songViewDto = _mapper.Map<SongViewDto>(song);

            return CreatedAtAction(nameof(GetSongById), new { id = songViewDto.Id }, songViewDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSong(long id, [FromForm] SongUpdateDto songUpdateDto)
    {
        try
        {
            var existingSong = await _repository.FindSongById(id);
            if (existingSong == null) return NotFound();
            if(songUpdateDto.CoverImage == null && (songUpdateDto.AudioFile == null && songUpdateDto.WavFile == null))
            {
                SongUpdateLessDto lessDto = new SongUpdateLessDto();
                var songDto = _mapper.Map(songUpdateDto, lessDto);
                var upSong = _mapper.Map(songDto, existingSong);
                await _repository.UpdateSong(upSong, null, null, null);
            }else if(songUpdateDto.CoverImage == null)
            {
                SongUpdateCoverLessDto lessDto = new SongUpdateCoverLessDto();
                var songDto = _mapper.Map(songUpdateDto, lessDto);
                var upSong = _mapper.Map(songDto, existingSong);
                await _repository.UpdateSong(upSong, songUpdateDto.AudioFile, songUpdateDto.WavFile, null);
            }else if(songUpdateDto.AudioFile == null && songUpdateDto.WavFile == null)
            {
                SongUpdateAudioLessDto lessDto = new SongUpdateAudioLessDto();
                var songDto = _mapper.Map(songUpdateDto, lessDto);
                var upSong = _mapper.Map(songDto, existingSong);
                await _repository.UpdateSong(upSong, null, null, songUpdateDto.CoverImage);
            }else
            {
                var updatedSong = _mapper.Map(songUpdateDto, existingSong);
                await _repository.UpdateSong(updatedSong, songUpdateDto.AudioFile, songUpdateDto.WavFile, songUpdateDto.CoverImage);
            }
            

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }

    [HttpPut("media")]
    public async Task<IActionResult> UpdateSongMedia(long songId, string mediaType, string media)
    {
        try
        {
            var result = await _repository.UpdateSongSocialMedia(songId, mediaType, media);
            var resultView = _mapper.Map<SongViewDto>(result);
            return Ok(resultView);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }
    
    [HttpPut("deactivate/{id}")]
    public async Task<IActionResult> DeactivateSongById(long id)
    {
        await _repository.DeactivateSongById(id);
        return NoContent();
    }
    
    [HttpPost("download")]
    public async Task<IActionResult> RecordDownload(long songId, long radioStationId)
    {
        await _repository.RecordDownloadAsync(songId, radioStationId);
        return Ok();
    }

    [HttpPost("play")]
    public async Task<IActionResult> RecordPlay(long songId, long radioStationId)
    {
        await _repository.RecordPlayAsync(songId, radioStationId);
        return Ok();
    }

    [HttpPost("visit")]
    public async Task<IActionResult> RecordVisit(long songId, long radioStationId)
    {
        await _repository.RecordSongVisits(songId, radioStationId);
        return Ok();
    }

    [HttpGet("song_visits")]
    public async Task<IActionResult> GetSongVisits(long songId)
    {
        var visits = await _repository.GetSongVisits(songId);
        return Ok(visits);
    }

    [HttpGet("suggest_number")]
    public async Task<IActionResult> GetSuggestNumbers(long songId)
    {
        var number = await _repository.GetSuggestNumbers(songId);
        return Ok(number);
    }

    [HttpGet("song_stats")]
    public async Task<IActionResult> GetSongStatistics()
    {
        var res = await _repository.GetSongStatisticsAsync();
        return Ok(res);
    }
    
    [HttpGet("allDownloads_artist")]
    public async Task<ActionResult<IEnumerable<Download>>> GetAllDownloads(long artistId)
    {
        var downloads = await _repository.GetAllDownloadsByArtist(artistId);
        return Ok(downloads);
    }

    [HttpGet("allPlays_artist")]
    public async Task<ActionResult<IEnumerable<Play>>> GetAllPlays(long artistId)
    {
        var plays = await _repository.GetAllPlaysByArtist(artistId);
        return Ok(plays);
    }
    
    [HttpGet("recommended/{radioStationId}")]
    public async Task<IActionResult> GetRecommendedSongs(long radioStationId)
    {
        var songs = await _repository.GetRecommendedSongsAsync(radioStationId);
        return Ok(songs);
    }

    [HttpGet("trending")]
    public async Task<IActionResult> GetTrendingSongs()
    {
        var songs = await _repository.GetTrendingSongsAsync();
        return Ok(songs);
    }

    [HttpGet("latest-added")]
    public async Task<ActionResult<IEnumerable<SongViewDto>>> GetLatestAddedSongs()
    {
        var songs = await _repository.GetLatestAddedSongsAsync();
        return Ok(_mapper.Map<IEnumerable<SongViewDto>>(songs));
    }

    [HttpGet("most-downloaded")]
    public async Task<IActionResult> GetMostDownloadedSongs()
    {
        var songs = await _repository.GetMostDownloadedSongsAsync();
        return Ok(songs);
    }

    [HttpGet("most-played")]
    public async Task<IActionResult> GetMostPlayedSongs()
    {
        var songs = await _repository.GetMostPlayedSongsAsync();
        return Ok(songs);
    }
    
    [HttpPost("account/{userId}/favorite/{songId}")]
    public async Task<IActionResult> AddFavoriteSong(string userId, long songId)
    {
        var result = await _repository.AddFavoriteSong(userId, songId);

        if (result)
        {
            return Ok(new { message = "Song added to favorites successfully." });
        }
        else
        {
            return BadRequest(new { message = "Song is already in favorites." });
        }
    }
    
    [HttpDelete("account/{userId}/removeFavorite/{songId}")]
    public async Task<IActionResult> RemoveFavoriteSong(string userId, long songId)
    {
        var result = await _repository.RemoveFavoriteSong(userId, songId);

        if (result)
        {
            return Ok(new { message = "Song removed from favorites successfully." });
        }
        else
        {
            return NotFound(new { message = "Song not found in favorites." });
        }
    }
    
    [HttpGet("account/{userId}/favorite/{songId}")]
    public async Task<IActionResult> IsFavoriteSong(string userId, long songId)
    {
        var isFavorite = await _repository.IsFavoriteSong(userId, songId);

        return Ok(new { isFavorite });
    }

    [HttpGet("by_decade")]
    public async Task<IActionResult> GetSongsByDecade()
    {
        var songs = await _repository.GetAllSongsByDecade();
        return Ok(songs);
    }

    [HttpGet("by_duration")]
    public async Task<IActionResult> GetSongsByDuration()
    {
        var songs = await _repository.GetAllSongsByDuration();
        return Ok(songs);
    }

    [HttpGet("accepts_by_decade")]
    public async Task<IActionResult> GetAcceptsByDecade()
    {
        var accepts = await _repository.GetAllAcceptsByDecade();
        return Ok(accepts);
    }

    [HttpGet("accepts_by_duration")]
    public async Task<IActionResult> GetAcceptsByDuration()
    {
        var accepts = await _repository.GetAllAcceptsByDuration();
        return Ok(accepts);
    }

    [HttpGet("top_genres")]
    public async Task<IActionResult> GetMostListnedTypes()
    {
        var result = await _repository.GetMostUsedTypes();
        return Ok(result);
    }

    [HttpGet("top_languages")]
    public async Task<IActionResult> GetMostUsedLanguages()
    {
        var result = await _repository.GetMostUsedLanguages();
        return Ok(result);
    }

    [HttpGet("plays_by_song")]
    public async Task<IActionResult> GetPlaysCountBySongId(long songId)
    {
        var result = await _repository.GetPlaysCountBySongId(songId);
        return Ok(result);
    }

    [HttpGet("downloads_by_song")]
    public async Task<IActionResult> GetDownloadsCountBySongId(long songId)
    {
        var result = await _repository.GetDownloadsCountBySongId(songId);
        return Ok(result);
    }

    [HttpGet("mensual_songs")]
    public async Task<IActionResult> GetMensualSongsByArtist(long artistId)
    {
        var result = await _repository.GetMensualSongsByArtist(artistId);
        return Ok(result);
    }

    [HttpGet("annual_songs")]
    public async Task<IActionResult> GetAnnualSongsByArtist(long artistId)
    {
        var result = await _repository.GetAnnualSongsByArtist(artistId);
        return Ok(result);
    }
    
    [HttpGet("search")]
    public async Task<IActionResult> SearchSongs(string query)
    {
        var songs = await _repository.SearchSongs(query);
        return Ok(_mapper.Map<IEnumerable<SongViewDto>>(songs));
    }
    
    [HttpPost("cosine-search/{id}")]
    public async Task<IActionResult> SearchSongsByCosine(long id)
    {
        try
        {
            var song = await _repository.GetSongById(id);
            if (song == null || string.IsNullOrEmpty(song.WavFilePath))
                return NotFound("Song or WAV file path not found");

            var results = await _repository.SearchSongsByAudio(song.WavFilePath, 5);
            
            var formattedResults = results.Select(r => new {
                Id = r.Item1,
                Score = r.Item2,
                Filename = r.Item3
            });

            return Ok(formattedResults);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error during cosine search: " + ex.Message);
        }
    }
    [HttpGet("recommended_by_favorites/{userId}")]
    public async Task<IActionResult> GetRecommendedSongsByFavorites(string userId, int limit = 10)
    {
        try
        {        
            var songs = await _repository.GetRecommendedSongsByFavoritesAsync(userId, limit);
            return Ok(_mapper.Map<IEnumerable<SongViewDto>>(songs));
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }
    
    [HttpPost("IAmessage")]
    public async Task<IActionResult> PostChatSong([FromBody] string message)
    {
        try
        {
            var songs = await _repository.PostChatSong(message);
            return Ok(_mapper.Map<IEnumerable<SongViewWithSimilarities>>(songs));
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }
    
    [HttpGet("monthly_listen_counts/{songId}")]
    public async Task<IActionResult> GetMonthlyListenCountsBySong(long songId)
    {
        try
        {
            var counts = await _repository.GetMonthlyListenCountsBySong(songId);
            return Ok(counts);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }
    
    [HttpPost("compare_favorites/{userId}/{songId}")]
    public async Task<IActionResult> CompareSongToUserFavorites(string userId, long songId)
    {
        try
        {
            var result = await _repository.CompareSongToUserFavoritesAsync(userId, songId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }
}

