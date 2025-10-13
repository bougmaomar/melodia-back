using System.Diagnostics;
using melodia_api.Entities;
using melodia_api.Exceptions;
using melodia_api.Models.Song;
using melodia.Configurations;
using melodia.Entities;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
//using FFMpegCore;
//using FFMpegCore.Enums;
//using FFMpegCore.Pipes;
using NAudio.Wave;
using TagLib;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Math;
using Microsoft.Extensions.Options;

namespace melodia_api.Repositories.Implementations;

public class SongRepository : ISongRepository
{
    private readonly MelodiaDbContext _db;
    private readonly string _fileDirectory;
    private readonly IQdrantRestClient _qdrantService;
    private readonly IAudioFeatureExtractor _audioFeatureExtractor;
    private readonly HttpClient _httpClient;
    private readonly string _urlIAchat;

    public SongRepository(MelodiaDbContext db, IConfiguration configuration,IQdrantRestClient qdrantService, IAudioFeatureExtractor audioFeatureExtractor,HttpClient httpClient, IOptions<DjangoSetting> apiOptions) { _db = db;   _fileDirectory = configuration["FileStorage:Directory"];_qdrantService = qdrantService;
        _audioFeatureExtractor = audioFeatureExtractor; _httpClient = httpClient;
        _urlIAchat = apiOptions.Value.BaseUrl;
    }
    
    private TimeSpan GetAudioDuration(string filePath)
    {
        var file = TagLib.File.Create(filePath);

        return file.Properties.Duration;
    }
    
    public async Task<Song> CreateSong(SongCreateDto songCreateDto)
    {
        var song = new Song
        {
            Title = songCreateDto.Title,
            ReleaseDate = songCreateDto.ReleaseDate,
            PlatformReleaseDate = songCreateDto.PlatformReleaseDate,
            GenreMusicId = songCreateDto.GenreMusicId,
            //LanguageId = songCreateDto.LanguageId,
            Lyrics = songCreateDto.Lyrics,
            CodeISRC = songCreateDto.CodeISRC,
            IsMapleMusic = songCreateDto.IsMapleMusic,
            IsMapleArtist = songCreateDto.IsMapleArtist,
            IsMaplePerformance = songCreateDto.IsMaplePerformance,
            IsMapleLyrics = songCreateDto.IsMapleLyrics,
            YouTube = songCreateDto.YouTube,
            Spotify = songCreateDto.Spotify,
            AlbumId = songCreateDto.AlbumId,
            SystemManage = songCreateDto.SystemManage,
            Active = true

        };
        
        string audioPathForEmbedding = null;

        if (songCreateDto.AudioFile != null)
        {
            song.Mp3FilePath = await UploadFileAsync(songCreateDto.AudioFile, "audio");
            var filePath = Path.Combine(_fileDirectory, song.Mp3FilePath);
            var duration = GetAudioDuration(filePath);
            song.Duration = $"{duration.Minutes}:{duration.Seconds:D2}"; // Format MM:SS
         }
        
        if (songCreateDto.WavFile != null)
        {
            song.WavFilePath = await UploadFileAsync(songCreateDto.WavFile, "audio");
            var filePath = Path.Combine(_fileDirectory, song.WavFilePath);
            audioPathForEmbedding = Path.Combine(_fileDirectory, song.WavFilePath);
            var duration = GetAudioDuration(filePath);
            song.Duration = $"{duration.Minutes}:{duration.Seconds:D2}"; // Format MM:SS
        }
        
        if (songCreateDto.CoverImage != null)
        {
            song.CoverImagePath = await UploadFileAsync(songCreateDto.CoverImage, "images");
        }
        
        ProcessSongRelationships(song, songCreateDto);

        _db.Songs.Add(song);
        await _db.SaveChangesAsync();
        
        if (songCreateDto.ArtistIds != null)
        {
            foreach (var artistId in songCreateDto.ArtistIds)
            {
                var songArtist = new SongArtist
                {
                    SongId = song.Id,
                    ArtistId = artistId,
                    Active = true
                };
                _db.SongArtists.Add(songArtist);
            }
            await _db.SaveChangesAsync();
        }
        
        if (audioPathForEmbedding != null)
        {
            var features = await _audioFeatureExtractor.ExtractVectorAsync(audioPathForEmbedding);
            var payload = new Dictionary<string, object>
            {
                { "filename", Path.GetFileName(audioPathForEmbedding) },
                { "label", song.GenreMusicId.ToString() }
            };
            await _qdrantService.UpsertAsync((int) song.Id, features, payload);
        }
        
        if (!string.IsNullOrEmpty(song.Lyrics) && !string.IsNullOrEmpty(song.WavFilePath))
        {
            var absoluteWavPath = Path.Combine(_fileDirectory, song.WavFilePath);

            var payload = new
            {
                id = song.Id,
                title = song.Title,
                lyrics = song.Lyrics,
                wav_file_path = absoluteWavPath
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(_urlIAchat + "/api/add/", content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error sending data to IAchat: {ex.Message}");
            }
        }
        
        
        var createdSong = await _db.Songs.Include(s => s.SongArtists)
            .ThenInclude(sa => sa.Artist)
            .ThenInclude(sa => sa.Account)
            .Include(s => s.Album)
            .Include(s => s.SongLanguages)
            .ThenInclude(sl => sl.Language)
            .Include(s => s.GenreMusic)
            .Include(s => s.SongComposers)
            .ThenInclude(sc => sc.Composer)
            .Include(s => s.SongWriters)
            .ThenInclude(sw => sw.Writer)
            .Include(s => s.SongCrOwners)
            .ThenInclude(sc => sc.CrOwner)
            .Where(s => s.Active == true)
            .FirstOrDefaultAsync(s => s.Id == song.Id);
        return createdSong;
    }
    
    private void ProcessSongRelationships(Song song, SongCreateDto songCreateDto)
    {
        song.SongComposers ??= new List<SongComposer>();
        song.SongWriters ??= new List<SongWriter>();
        song.SongCrOwners ??= new List<SongCROwner>();
        song.SongLanguages ??= new List<SongLanguages>();

        foreach (var composerId in songCreateDto.ComposerIds)
        {
            var songComposer = new SongComposer { ComposerId = composerId, SongId = song.Id };
            song.SongComposers.Add(songComposer);
        }

        foreach (var writerId in songCreateDto.WriterIds)
        {
            var songWriter = new SongWriter { WriterId = writerId, SongId = song.Id };
            song.SongWriters.Add(songWriter);
        }

        foreach (var crOwnerId in songCreateDto.CopyrightOwnerIds)
        {
            var songCROwner = new SongCROwner { CROwnerId = crOwnerId, SongId = song.Id };
            song.SongCrOwners.Add(songCROwner);
        }

        foreach (var languageId in songCreateDto.LanguageIds)
        {
            var songLanguage = new SongLanguages { LanguageId = languageId, SongId = song.Id };
            song.SongLanguages.Add(songLanguage);
        }

        _db.SaveChangesAsync();
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
    
    public async Task<Song> UpdateSong(Song updatedSong, IFormFile newAudioFile , IFormFile newWavFile , IFormFile newCoverImage)
    {
        bool shouldReindex = false;
        string audioPath = null;
        var song = await _db.Songs.FindAsync(updatedSong.Id);
        if (song == null)
        {
            throw new Exception("Song not found");
        }

        _db.Entry(song).CurrentValues.SetValues(updatedSong);

        if (newAudioFile != null)
        {
            if (!string.IsNullOrEmpty(song.Mp3FilePath))
            {
                DeleteFile(song.Mp3FilePath); 
            }
            song.Mp3FilePath = await UploadFileAsync(newAudioFile, "audio");
            var filePath = Path.Combine(_fileDirectory, song.Mp3FilePath);
            var duration = GetAudioDuration(filePath);
            song.Duration = $"{duration.Minutes}:{duration.Seconds:D2}"; // Format MM:SS
        }

        if (newWavFile != null)
        {
            if (!string.IsNullOrEmpty(song.WavFilePath))
            {
                DeleteFile(song.WavFilePath); 
            }
            audioPath = Path.Combine(_fileDirectory, song.WavFilePath);

            song.WavFilePath = await UploadFileAsync(newWavFile, "audio");
            shouldReindex = true;
        }

        if (newCoverImage != null)
        {
            if (!string.IsNullOrEmpty(song.CoverImagePath))
            {
                DeleteFile(song.CoverImagePath); // Supprimez l'ancien fichier
            }
            song.CoverImagePath = await UploadFileAsync(newCoverImage, "images");
        }

        await _db.SaveChangesAsync();
        
        if (shouldReindex && !string.IsNullOrEmpty(audioPath))
        {
            await _qdrantService.DeletePointAsync((int)song.Id);

            var vector = await _audioFeatureExtractor.ExtractVectorAsync(audioPath);
            var payload = new Dictionary<string, object>
            {
                { "filename", Path.GetFileName(audioPath) },
                { "label", song.GenreMusicId.ToString() }
            };

            await _qdrantService.UpsertAsync((int)song.Id, vector, payload);
        }
        return song;
    }

    public async Task<Song> UpdateSongSocialMedia(long songId, string mediaType, string media)
    {
        var song = await _db.Songs.FindAsync(songId);
        if (song == null)
        {
            throw new Exception("Song not found");
        }

        if (mediaType == "Spotify")
        {
            if (media == "null") song.Spotify = null;
            else song.Spotify = media;
            
            _db.Songs.Update(song); 
            await _db.SaveChangesAsync();
        }

        if (mediaType == "Youtube")
        {
            if (media == "null") song.YouTube = null;
            else song.YouTube = media; 

            _db.Songs.Update(song);
            await _db.SaveChangesAsync();
        }

        return song;
    }

    private void DeleteFile(string filePath)
    {
        var fullPath = Path.Combine(_fileDirectory, filePath);
        if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
    }
    
    public async Task DeactivateSongById(long id)
    {
        var song = await _db.Songs.FindAsync(id);
        if (song != null)
        {
            song.Active = false;
            await _db.SaveChangesAsync();
        }
        await _qdrantService.DeletePointAsync((int) id);
        
    }
    
    public async Task ActivateSongById(long id)
    {
        var song = await _db.Songs.FindAsync(id);
        if (song != null)
        {
            song.Active = true;
            await _db.SaveChangesAsync();
        }
    }
    
    public async Task<List<Song>> GetAllSongs()
    {
        return await _db.Songs
            .Include(s => s.SongArtists) 
            .ThenInclude(sa => sa.Artist)
            .ThenInclude(sa => sa.Account)
            .Include(s => s.Album)
            .Include(s => s.SongLanguages)
            .ThenInclude(sl => sl.Language)
            .Include(s => s.GenreMusic)
            .Include(s => s.SongComposers)
            .ThenInclude(sc => sc.Composer)
            .Include(s => s.SongWriters)
            .ThenInclude(sw => sw.Writer)
            .Include(s => s.SongCrOwners)
            .ThenInclude(sc => sc.CrOwner)
            .Where(s => s.Active == true)
            .ToListAsync();
    }

    public async Task<List<Song>> GetSongsForStation()
    {
        return await _db.Songs
            .Include(s => s.SongArtists)
            .ThenInclude(sa => sa.Artist)
            .ThenInclude(sa => sa.Account)
            .Include(s => s.Album)
            .Include(s => s.SongLanguages)
            .ThenInclude(sl => sl.Language)
            .Include(s => s.GenreMusic)
            .Include(s => s.SongComposers)
            .ThenInclude(sc => sc.Composer)
            .Include(s => s.SongWriters)
            .ThenInclude(sw => sw.Writer)
            .Include(s => s.SongCrOwners)
            .ThenInclude(sc => sc.CrOwner)
            .Where(s => s.Active == true && s.SystemManage == true)
            .ToListAsync();
    }
    
    public async Task<List<Song>> GetFavoriteSongs(string userId)
    {
        return await _db.Songs
            .Include(s => s.FavoriteSongs)
            .ThenInclude(sa => sa.Account)
            .Include(s => s.Album)
            .Include(s => s.SongArtists)
            .ThenInclude(sa => sa.Artist)
            .ThenInclude(saa => saa.Account)
            .Include(s => s.SongLanguages)
            .ThenInclude(sl => sl.Language)
            .Include(s => s.GenreMusic)
            .Include(s => s.SongComposers)
            .ThenInclude(sc => sc.Composer)
            .Include(s => s.SongWriters)
            .ThenInclude(sw => sw.Writer)
            .Include(s => s.SongCrOwners)
            .ThenInclude(sc => sc.CrOwner)
            .Where(s => s.Active == true)
            .Where(s => s.FavoriteSongs.Any(aa => aa.UserId == userId))
            .ToListAsync();
            

    }
    
    public async Task<Song> FindSongById(long id)
    {
        return await _db.Songs
            .Include(s => s.SongArtists)
            .ThenInclude(sa => sa.Artist)
            .ThenInclude(sa => sa.Account)
            .Include(s => s.Album)
            .Include(s => s.SongLanguages)
            .ThenInclude(sl => sl.Language)
            .Include(s => s.GenreMusic)
            .Include(s => s.SongComposers)
            .ThenInclude(sc => sc.Composer)
            .Include(s => s.SongWriters)
            .ThenInclude(sw => sw.Writer)
            .Include(s => s.SongCrOwners)
            .ThenInclude(sc => sc.CrOwner)
            .Where(s => s.Active == true)
            .FirstOrDefaultAsync(s => s.Id == id);
        
    }

    public async Task<Song> GetSongById(long id)
    {
        return await _db.Songs
            .Include(s => s.SongArtists)
            .ThenInclude(sa => sa.Artist)
            .ThenInclude(sa => sa.Account)
            .Include(s => s.Album)
            .Include(s => s.SongLanguages)
            .ThenInclude(sl => sl.Language)
            .Include(s => s.GenreMusic)
            .Include(s => s.SongComposers)
            .ThenInclude(sc => sc.Composer)
            .Include(s => s.SongWriters)
            .ThenInclude(sw => sw.Writer)
            .Include(s => s.SongCrOwners)
            .ThenInclude(sc => sc.CrOwner)
            .Where(s => s.Active == true)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Song>> FindSongByArtistEmail(string email)
    {
        return await _db.Songs
            .Include(s => s.Album)
            .Include(s => s.SongArtists)
            .ThenInclude(sa => sa.Artist)
            .ThenInclude(sa => sa.Account)
            .Include(s => s.SongLanguages)
            .ThenInclude(sl => sl.Language)
            .Include(s => s.GenreMusic)
            .Include(s => s.SongComposers)
            .ThenInclude(sc => sc.Composer)
            .Include(s => s.SongWriters)
            .ThenInclude(sw => sw.Writer)
            .Include(s => s.SongCrOwners)
            .ThenInclude(sc => sc.CrOwner)
            .Where(s => s.SongArtists.Any(sa => sa.Artist.Account.Email == email && sa.Active == true))  
            .Where(s => s.Active == true)
            //.Where(s => s.Album.Active == true)
            .ToListAsync();
    }
    
    public async Task<List<Song>> FindSongsByLanguage(long languageId)
    {
        return await _db.Songs
            .Include(s => s.Album)
            .Include(s => s.SongArtists)
            .ThenInclude(sa => sa.Artist)
            .ThenInclude(saa => saa.Account)
            .Include(s => s.SongLanguages)
            .ThenInclude(sl => sl.Language)
            .Include(s => s.GenreMusic)
            .Include(s => s.SongComposers)
            .ThenInclude(sc => sc.Composer)
            .Include(s => s.SongWriters)
            .ThenInclude(sw => sw.Writer)
            .Include(s => s.SongCrOwners)
            .ThenInclude(sc => sc.CrOwner)
            .Where(s => s.SongLanguages.Any(sl => sl.Language.Id == languageId && sl.Active == true))
            .Where(s => s.Active == true)
            .ToListAsync();
    }
    
    public async Task<List<Song>> FindSongsByType(long typeId)
    {
        return await _db.Songs
            .Include(s => s.Album)
            .Include(s => s.SongArtists)
            .ThenInclude(sa => sa.Artist)
            .ThenInclude(saa => saa.Account)
            .Include(s => s.SongLanguages)
            .ThenInclude(sl => sl.Language)
            .Include(s => s.GenreMusic)
            .Include(s => s.SongComposers)
            .ThenInclude(sc => sc.Composer)
            .Include(s => s.SongWriters)
            .ThenInclude(sw => sw.Writer)
            .Include(s => s.SongCrOwners)
            .ThenInclude(sc => sc.CrOwner)
            .Where(s => s.GenreMusic.Id == typeId)
            .Where(s => s.Active == true)
            .ToListAsync();
    }

    public async Task<List<Song>> FindSongsByArtist(long artistId)
    {
        return await _db.Songs
            .Include(s => s.Album)
            .Include(s => s.SongLanguages)
            .ThenInclude(sl => sl.Language)
            .Include(s => s.GenreMusic)
            .Include(s => s.SongArtists)
            .ThenInclude(sa => sa.Artist)
            .ThenInclude(saa => saa.Account)
            .Include(s => s.SongComposers)
            .ThenInclude(sc => sc.Composer)
            .Include(s => s.SongWriters)
            .ThenInclude(sw => sw.Writer)
            .Include(s => s.SongCrOwners)
            .ThenInclude(sc => sc.CrOwner)
            .Where(s => s.SongArtists.Any(sa => sa.Artist.Id == artistId))
            .Where(s => s.Active == true)
            .ToListAsync();
    }

    public async Task RecordDownloadAsync(long songId, long radioStationId)
    {
        var download = new Download
        {
            SongId = songId,
            RadioStationId = radioStationId
        };

        _db.Downloads.Add(download);
        await _db.SaveChangesAsync();
    }

    public async Task RecordSongVisits(long songId, long radioStationId)
    {
        var existing = await _db.SongVisits.Where(s => s.SongId == songId && s.RadioStationId == radioStationId).FirstOrDefaultAsync();
        if (existing != null)
        {
            existing.VisitNumber++;
            _db.Update(existing);
            await _db.SaveChangesAsync();
        }
        else
        {
            var visit = new SongVisit()
            {
                SongId = songId,
                RadioStationId = radioStationId,
                VisitNumber = 1
            };
            _db.Add(visit);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<int> GetSuggestNumbers(long songId)
    {
        var number = await _db.Proposals.Where(s => s.SongId == songId).CountAsync();
        return number;
    }

    public async Task<int> GetSongVisits(long songId)
    {
        var songs = await _db.SongVisits.Where(s => s.SongId == songId).ToListAsync();
        var number = 0;
        foreach (var song in songs)
        {
            number += song.VisitNumber;
        }

        return number;
    }

    public async Task<List<Download>> GetAllDownloadsByArtist(long artistId)
    {
        return await _db.Downloads
            .Include(d => d.Song)
            .Include(d => d.RadioStation)
            .Where(d => d.Song.SongArtists.Any(ss => ss.ArtistId == artistId))
            .ToListAsync();
    }

    public async Task<List<Play>> GetAllPlaysByArtist(long artistId)
    {
        return await _db.Plays
            .Include(p => p.Song)
            .Include(p => p.RadioStation)
            .Where(p => p.Song.SongArtists.Any(ps => ps.ArtistId == artistId))
            .ToListAsync();
    }

    public async Task<int> GetSongPlays(long songId)
    {
        return await _db.Plays.Where(p => p.SongId == songId).CountAsync();
    }

    public async Task<int> GetSongDownloads(long songId)
    {
        return await _db.Downloads.Where(d => d.SongId == songId).CountAsync();
    }
    
    public async Task RecordPlayAsync(long songId, long radioStationId)
    {
        var play = new Play
        {
            SongId = songId,
            RadioStationId = radioStationId
        };

        _db.Plays.Add(play);
        await _db.SaveChangesAsync();
    }

    public async Task<List<Song>> GetRelatedSongs(long songId)
    {
        var artistIds = await _db.Songs
            .Where(s => s.Id == songId)
            .SelectMany(s => s.SongArtists.Select(sa => sa.ArtistId))
            .ToListAsync();

        var relatedSongs = await _db.Songs
            .Include(s => s.SongArtists)
            .ThenInclude(sa => sa.Artist)
            .ThenInclude(sa => sa.Account)
            .Include(s => s.Album)
            .Include(s => s.SongLanguages)
            .ThenInclude(sl => sl.Language)
            .Include(s => s.GenreMusic)
            .Include(s => s.SongComposers)
            .ThenInclude(sc => sc.Composer)
            .Include(s => s.SongWriters)
            .ThenInclude(sw => sw.Writer)
            .Include(s => s.SongCrOwners)
            .ThenInclude(sc => sc.CrOwner)
            .Where(s => s.Id != songId && s.SongArtists.Any(sa => artistIds.Contains(sa.ArtistId)))
            .Where(s => s.Active == true)
            .ToListAsync();

        return relatedSongs;
    }

    public async Task<List<Song>> GetRecommendedSongsAsync(long radioStationId) 
    { 
        var acceptedSongs = await _db.Proposals
            .Where(p => p.RadioStationId == radioStationId && p.Status == ProposalStatus.Accepted)
            .Select(p => p.Song)
            .ToListAsync(); 
        
        if (!acceptedSongs.Any()) return new List<Song>();
        
        var genreIds = acceptedSongs.Select(s => s.GenreMusicId).Distinct(); 
        var languageIds = acceptedSongs.Select(s => s.SongLanguages.Select(sl => sl.LanguageId)).Distinct();
        
        var recommendedSongs = await _db.Songs
            .Include(s => s.Album)
            .Include(s => s.SongLanguages).ThenInclude(sl => sl.Language)
            .Include(s => s.GenreMusic)
            .Include(s => s.SongComposers).ThenInclude(sc => sc.Composer)
            .Include(s => s.SongWriters).ThenInclude(sw => sw.Writer)
            .Include(s => s.SongCrOwners).ThenInclude(sc => sc.CrOwner)
            .Include(s => s.SongArtists).ThenInclude(sa => sa.Artist).ThenInclude(a => a.Account)
            .Where(s => genreIds.Contains(s.GenreMusicId) && languageIds.Contains(s.SongLanguages.Select(sl => sl.LanguageId)))
            .Where(s => s.Active)
            .Where(s => s.Album.Active)
            .ToListAsync();
        
        return recommendedSongs;
    }

    public async Task<List<Song>> GetTrendingSongsAsync()
    {
        var acceptedProposals = await _db.Proposals
            .Where(p => p.Status == ProposalStatus.Accepted)
            .Include(p => p.Song)
            .ToListAsync();

        var trendingSongs = acceptedProposals
            .GroupBy(p => p.Song)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .Take(10)
            .ToList();

        var result = await _db.Songs
            .Include(s => s.SongArtists).ThenInclude(sa => sa.Artist).ThenInclude(a => a.Account)
            .Include(s => s.Album)
            .Include(s => s.SongLanguages).ThenInclude(sl => sl.Language)
            .Include(s => s.GenreMusic)
            .Include(s => s.SongComposers).ThenInclude(sc => sc.Composer)
            .Include(s => s.SongWriters).ThenInclude(sw => sw.Writer)
            .Include(s => s.SongCrOwners).ThenInclude(sc => sc.CrOwner)
            .Where(s => trendingSongs.Select(ts => ts.Id).Contains(s.Id) && s.Active)
            .ToListAsync();

        return result;
    }

    public async Task<int> GetSongsByYear(int year)
    {
        var startYear = year;
        var endYear = year + 9;

        var songs = await _db.Songs
            .Where(song => song.ReleaseDate.Year >= startYear && song.ReleaseDate.Year <= endYear)
            .ToListAsync();

        return songs.Count;
    }

    public async Task<int> GetAcceptsByYear(int year)
    {
        var startYear = year;
        var endYear = year + 9;

        var accepts = await _db.Proposals
            .Where(p => p.Status == ProposalStatus.Accepted && (p.Song.ReleaseDate.Year >= startYear && p.Song.ReleaseDate.Year <= endYear))
            .ToListAsync();

        return accepts.Count;
    }

    public async Task<int> GetSongsByDuration(int minDuration, bool isLastDuration = false)
    {
        var startDuration = minDuration;

        var songs = await _db.Songs
            .Where(song => song.Duration != null)
            .ToListAsync();

        var filteredSongs = songs
            .Where(song =>
            {
                TimeSpan duration;
                if (TimeSpan.TryParse(song.Duration, out duration))
                {
                    if (isLastDuration)
                    {
                        return duration.Hours >= startDuration;
                    }
                    else
                    {
                        return duration.Hours >= startDuration && duration.Hours < startDuration + 1;
                    }
                }
                return false;
            })
            .ToList();

        return filteredSongs.Count;
    }

    public async Task<int> GetAcceptsByDuration(int minDuration, bool isLastDuration =false)
    {
        var startDuration = minDuration;
        var accepts = await _db.Proposals
            .Include(p => p.Song)
            .Where(p => p.Status == ProposalStatus.Accepted)
            .ToListAsync();

        var filteredSongs = accepts
            .Where(accept =>
            {
                TimeSpan duration;
                if (TimeSpan.TryParse(accept.Song.Duration, out duration))
                {
                    if (isLastDuration)
                    {
                        return duration.Hours >= startDuration;
                    }
                    else
                    {
                        return duration.Hours >= startDuration && duration.Hours < startDuration + 1;
                    }
                }
                return false;
            })
            .ToList();

        return filteredSongs.Count;
    }

    public async Task<int> GetSongStatisticsAsync()
    {
        var currentDate = DateTime.UtcNow;
        var firstDayOfCurrentMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
        var firstDayOfLastMonth = firstDayOfCurrentMonth.AddMonths(-1);
        var lastDayOfLastMonth = firstDayOfCurrentMonth.AddDays(-1);

        var currentMonthCount = await _db.Songs
            .Where(s => s.PlatformReleaseDate >= firstDayOfCurrentMonth)
            .CountAsync();

        var lastMonthCount = await _db.Songs
            .Where(s => s.PlatformReleaseDate >= firstDayOfLastMonth && s.PlatformReleaseDate <= lastDayOfLastMonth)
            .CountAsync();

        return currentMonthCount - lastMonthCount;

    }
    
    public async Task<List<KeyValuePair<int, int>>> GetAllSongsByDecade()
    {
        var years = new List<int> { 1970, 1980, 1990, 2000, 2010, 2020 };
        var result = new List<KeyValuePair<int, int>>();

        foreach (var year in years)
        {
            var number = await GetSongsByYear(year);
            result.Add(new KeyValuePair<int, int>(year, number));
        }

        return result;
    }

    public async Task<List<KeyValuePair<int, int>>> GetAllAcceptsByDecade()
    {
        var years = new List<int> { 1970, 1980, 1990, 2000, 2010, 2020 };
        var result = new List<KeyValuePair<int, int>>();

        foreach (var year in years)
        {
            var number = await GetAcceptsByYear(year);
            result.Add(new KeyValuePair<int, int>(year, number));
        }

        return result;
    }

    public async Task<List<KeyValuePair<int, int>>> GetAllSongsByDuration()
    {
        var durations = new List<int> { 0, 1, 2, 3, 4, 5 };
        var result = new List<KeyValuePair<int, int>>();

        for (int i = 0; i < durations.Count; i++)
        {
            var duration = durations[i];

            bool isLastDuration = (i == durations.Count - 1);

            var number = await GetSongsByDuration(duration, isLastDuration);
            result.Add(new KeyValuePair<int, int>(duration, number));
        }

        return result;
    }

    public async Task<List<KeyValuePair<int, int>>> GetAllAcceptsByDuration()
    {
        var durations = new List<int> { 0, 1, 2, 3, 4, 5 };
        var result = new List<KeyValuePair<int, int>>();

        for (int i = 0; i < durations.Count; i++)
        {
            var duration = durations[i];

            bool isLastDuration = (i == durations.Count - 1);

            var number = await GetAcceptsByDuration(duration, isLastDuration);
            result.Add(new KeyValuePair<int, int>(duration, number));
        }

        return result;
    }

    public async Task<List<KeyValuePair<string, int>>> GetMostUsedTypes()
    {
        var songsCount = await _db.Songs.CountAsync();
        var result = new List<KeyValuePair<string, int>>();

        if (songsCount == 0)
        {
            return result;
        }

        var genreCounts = await _db.GenreMusics
            .Select(g => new
            {
                Genre = g.Name,
                Count = _db.Songs.Count(s => s.GenreMusic.Name == g.Name)
            })
            .ToListAsync();

        genreCounts = genreCounts.OrderByDescending(gc => gc.Count).ToList();

        var topGenres = genreCounts.Take(4).ToList();
        var othersCount = genreCounts.Skip(4).Sum(gc => gc.Count);

        foreach (var genre in topGenres)
        {
            var typePercentage = genre.Count * 100 / songsCount;
            result.Add(new KeyValuePair<string, int>(genre.Genre, typePercentage));
        }

        if (othersCount > 0)
        {
            var othersPercentage = othersCount * 100 / songsCount;
            result.Add(new KeyValuePair<string, int>("Others", othersPercentage));
        }

        return result;
    }


    public async Task<List<KeyValuePair<string, int>>> GetMostUsedLanguages()
    {
        var songsCount = await _db.Songs.CountAsync();
        var result = new List<KeyValuePair<string, int>>();

        if (songsCount == 0)
        {
            return result;
        }
        
        var languagesCounts = await _db.Languages
            .Select(g => new
            {
                Language = g.Label,
                Count = _db.Songs.Count(s => s.SongLanguages.Any(l => l.Language.Label == g.Label))
            })
            .ToListAsync();

        languagesCounts = languagesCounts.OrderByDescending(gc => gc.Count).ToList();

        var topLanguages = languagesCounts.Take(4).ToList();
        var othersCount = languagesCounts.Skip(4).Sum(gc => gc.Count);

        foreach (var lang in topLanguages)
        {
            var typePercentage = lang.Count * 100 / songsCount;
            result.Add(new KeyValuePair<string, int>(lang.Language, typePercentage));
        }

        if (othersCount > 0)
        {
            var othersPercentage = othersCount * 100 / songsCount;
            result.Add(new KeyValuePair<string, int>("Others", othersPercentage));
        }

        return result;
    }
    
    public async Task<List<Song>> GetLatestAddedSongsAsync()
    {
        return await _db.Songs
            .Include(s => s.SongArtists)
            .ThenInclude(sa => sa.Artist)
            .ThenInclude(sa => sa.Account)
            .Include(s => s.Album)
            .Include(s => s.SongLanguages)
            .ThenInclude(sl => sl.Language)
            .Include(s => s.GenreMusic)
            .Include(s => s.SongComposers)
            .ThenInclude(sc => sc.Composer)
            .Include(s => s.SongWriters)
            .ThenInclude(sw => sw.Writer)
            .Include(s => s.SongCrOwners)
            .ThenInclude(sc => sc.CrOwner)
            .Where(s => s.Active == true)
            .Where(s => s.Album.Active == true)
            .Take(10)
            .ToListAsync();
    }

    public async Task<List<Song>> GetMostDownloadedSongsAsync()
{
    var downloads = await _db.Downloads
        .ToListAsync();

    var mostDownloadedSongIds = downloads
        .GroupBy(d => d.SongId)
        .OrderByDescending(g => g.Count())
        .Select(g => g.Key)
        .Take(10)
        .ToList();

    var mostDownloadedSongs = await _db.Songs
        .Include(s => s.Album)
        .Include(s => s.SongLanguages)
        .ThenInclude(sl => sl.Language)
        .Include(s => s.GenreMusic)
        .Include(s => s.SongComposers).ThenInclude(sc => sc.Composer)
        .Include(s => s.SongWriters).ThenInclude(sw => sw.Writer)
        .Include(s => s.SongCrOwners).ThenInclude(sc => sc.CrOwner)
        .Include(s => s.SongArtists).ThenInclude(sa => sa.Artist).ThenInclude(a => a.Account)
        .Where(s => mostDownloadedSongIds.Contains(s.Id))
        .Where(s => s.Active)
        .Where(s => s.Album.Active)
        .ToListAsync();

    return mostDownloadedSongs;
}

    public async Task<List<Song>> GetMostPlayedSongsAsync()
{
    var plays = await _db.Plays
        .ToListAsync();

    var mostPlayedSongIds = plays
        .GroupBy(p => p.SongId)
        .OrderByDescending(g => g.Count())
        .Select(g => g.Key)
        .Take(10)
        .ToList();

    var mostPlayedSongs = await _db.Songs
        .Include(s => s.Album)
        .Include(s => s.SongLanguages).ThenInclude(sl => sl.Language)
        .Include(s => s.GenreMusic)
        .Include(s => s.SongComposers).ThenInclude(sc => sc.Composer)
        .Include(s => s.SongWriters).ThenInclude(sw => sw.Writer)
        .Include(s => s.SongCrOwners).ThenInclude(sc => sc.CrOwner)
        .Include(s => s.SongArtists).ThenInclude(sa => sa.Artist).ThenInclude(a => a.Account)
        .Where(s => mostPlayedSongIds.Contains(s.Id))
        .Where(s => s.Active)
        .Where(s => s.Album.Active)
        .ToListAsync();

    return mostPlayedSongs;
}

    public async Task<long> GetPlaysCountBySongId(long songId)
    {
        var plays = await _db.Plays
            .Where(p => p.SongId == songId)
            .ToListAsync();
        return plays.Count();
    }

    public async Task<long> GetDownloadsCountBySongId(long songId)
    {
        var downloads = await _db.Downloads
            .Where(d => d.SongId == songId)
            .ToListAsync();
        return downloads.Count();
    }
    
    public async Task<bool> AddFavoriteSong(string userId, long songId)
{
    var existingFavorite = await _db.FavoriteSongs
        .FirstOrDefaultAsync(f => f.UserId == userId && f.SongId == songId);

    if (existingFavorite != null)
    {
        return false;
    }

    var favoriteSong = new FavoriteSongs
    {
        UserId = userId,
        SongId = songId
    };

    _db.FavoriteSongs.Add(favoriteSong);
    
    var saved = await _db.SaveChangesAsync();

    return saved > 0;
}

    public async Task<bool> RemoveFavoriteSong(string userId, long songId)
{
    var favoriteSong = await _db.FavoriteSongs
        .FirstOrDefaultAsync(f => f.UserId == userId && f.SongId == songId);

    if (favoriteSong == null)
    {
        return false; // Song is not in the user's favorites
    }

    _db.FavoriteSongs.Remove(favoriteSong);

    var removed = await _db.SaveChangesAsync();

    return removed > 0;
}

    public async Task<bool> IsFavoriteSong(string userId, long songId)
{
    return await _db.FavoriteSongs
        .AnyAsync(f => f.UserId == userId && f.SongId == songId);
}

    public async Task<int> GetTrendingSongsCountAsync()
    {
        return (await GetTrendingSongsAsync()).Count;
    }

    // Nombre de dernières chansons ajoutées
    public async Task<int> GetLatestAddedSongsCountAsync()
    {
        return (await GetLatestAddedSongsAsync()).Count;
    }

    // Nombre de chansons les plus téléchargées
    public async Task<int> GetMostDownloadedSongsCountAsync()
    {
        return (await GetMostDownloadedSongsAsync()).Count;
    }

    // Nombre de chansons les plus jouées
    public async Task<int> GetMostPlayedSongsCountAsync()
    {
        return (await GetMostPlayedSongsAsync()).Count;
    }

    // Nombre total de chansons
    public async Task<int> GetTotalSongsCountAsync()
    {
        return await _db.Songs.CountAsync();
    }
    
    public async Task<int> GetSinglesCountAsync()
    {
        return await _db.Songs.CountAsync(s => s.AlbumId == null);
    }

    public async Task<int[]> GetMensualSongsByArtist(long artistId)
    {
        var currentYear = DateTime.Now.Year;

        int[] monthlySongs = new int[12];

        var songs = _db.Songs
            .Where(s => s.SongArtists.Any(sa => sa.ArtistId == artistId) && s.PlatformReleaseDate.Value.Year == currentYear)
            .ToList();

        foreach (var song in songs)
        {
            int monthIndex = song.PlatformReleaseDate.Value.Month - 1;
            monthlySongs[monthIndex]++;
        }

        return monthlySongs;
    }

    public async Task<int[]> GetAnnualSongsByArtist(long artistId)
    {
        int[] annualSongs = new int[7];
        int currentYear = DateTime.Now.Year;

        var songs = _db.Songs
            .Where(s => s.SongArtists.Any(sa => sa.ArtistId == artistId) && s.PlatformReleaseDate.Value.Year >= currentYear - 6)
            .ToList();

        foreach (var song in songs)
        {
            int year = song.PlatformReleaseDate.Value.Year;
            int yearIndex = currentYear - year;

            if (yearIndex >= 0 && yearIndex < 7)
            {
                annualSongs[yearIndex]++;
            }
        }

        return annualSongs.Reverse().ToArray();
    }
    
    public async Task<List<Song>> SearchSongs(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return new List<Song>();

        searchTerm = searchTerm.ToLower();

        return await _db.Songs
            .Include(s => s.Album)
            .Include(s => s.SongArtists).ThenInclude(sa => sa.Artist).ThenInclude(a => a.Account)
            .Include(s => s.SongLanguages).ThenInclude(sl => sl.Language)
            .Include(s => s.GenreMusic)
            .Include(s => s.SongComposers).ThenInclude(sc => sc.Composer)
            .Include(s => s.SongWriters).ThenInclude(sw => sw.Writer)
            .Include(s => s.SongCrOwners).ThenInclude(sc => sc.CrOwner)
            .Where(s => s.Active &&
                        (
                            EF.Functions.Like(s.Title.ToLower(), $"%{searchTerm}%") ||
                            EF.Functions.Like(s.Lyrics.ToLower(), $"%{searchTerm}%") ||
                            EF.Functions.Like(s.CodeISRC.ToLower(), $"%{searchTerm}%") ||
                            EF.Functions.Like(s.Spotify.ToLower(), $"%{searchTerm}%") ||
                            EF.Functions.Like(s.YouTube.ToLower(), $"%{searchTerm}%") ||
                            EF.Functions.Like(s.GenreMusic.Name.ToLower(), $"%{searchTerm}%") ||
                            s.SongArtists.Any(sa => EF.Functions.Like(sa.Artist.Name.ToLower(), $"%{searchTerm}%")) ||
                            s.SongLanguages.Any(sl => EF.Functions.Like(sl.Language.Label.ToLower(), $"%{searchTerm}%"))
                        ))
            .ToListAsync();
    }
    
    public async Task<List<(int, float, string)>> SearchSongsByAudio(string audioFilePath, int limit = 5)
    {
        var filePath = Path.Combine(_fileDirectory, audioFilePath);
        var vector = await  _audioFeatureExtractor.ExtractVectorAsync(filePath);
        return await _qdrantService.SearchAsync(vector, limit);
    }
    
    public async Task<List<Song>> GetRecommendedSongsByFavoritesAsync(string userId, int limit = 10)
    {
        const int maxLikesUsed = 20; // Nombre maximum de chansons aimées à utiliser pour la recommandation
        
        var likedSongs = (await GetFavoriteSongs(userId))
            .Where(s => !string.IsNullOrEmpty(s.WavFilePath) || !string.IsNullOrEmpty(s.Mp3FilePath))
            .OrderByDescending(s => s.ReleaseDate)
            .Take(maxLikesUsed)
            .ToList();
        
        if (!likedSongs.Any()) return  await GetTrendingSongsAsync();
        
        var resultScores = new Dictionary<long, float>();

        foreach (var song in likedSongs)
        {
            var filePath = song.WavFilePath ?? song.Mp3FilePath;
            if (string.IsNullOrEmpty(filePath)) continue;

            var fullPath = Path.Combine(_fileDirectory, filePath);
            var vector = await _audioFeatureExtractor.ExtractVectorAsync(fullPath);

            var similarSongs = await _qdrantService.SearchAsync(vector, 15);

            foreach (var (id, score, _) in similarSongs)
            {
                if (id == song.Id) continue; // éviter les doublons directs
                if (resultScores.ContainsKey(id))
                    resultScores[id] += score;
                else
                    resultScores[id] = score;
            }
        }

        var bestSongIds = resultScores
            .OrderByDescending(kv => kv.Value)
            .Select(kv => kv.Key)
            .Take(limit)
            .ToList();

        var recommendedSongs = await _db.Songs
            .Include(s => s.Album)
            .Include(s => s.SongLanguages).ThenInclude(sl => sl.Language)
            .Include(s => s.GenreMusic)
            .Include(s => s.SongArtists).ThenInclude(sa => sa.Artist).ThenInclude(a => a.Account)
            .Where(s => bestSongIds.Contains(s.Id) && s.Active)
            .ToListAsync();

        // Pour garder le même ordre que le score
        return recommendedSongs.OrderBy(s => bestSongIds.IndexOf(s.Id)).ToList();
    }

    public async Task<List<SongViewWithSimilarities>> PostChatSong(string message)
    {
        var payload = new
        {
            query = message
        };

        var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json"
        );

        try
        {
            var response = await _httpClient.PostAsync(_urlIAchat + "/api/recommend/", content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;
            if (!root.TryGetProperty("results", out var resultsArray) || resultsArray.ValueKind != JsonValueKind.Array)
                return new List<SongViewWithSimilarities>();

            // Dictionnaire ID => % Similarité
            var idSimilarityMap = new Dictionary<long, float>();

            foreach (var item in resultsArray.EnumerateArray())
            {
                if (!item.TryGetProperty("id", out var idProp)) continue;

                var id = idProp.GetInt64();
                var similarity = item.GetProperty("similarity").GetSingle();
                idSimilarityMap[id] = (float)Math.Round(similarity * 100, 2);
            }

            var songIds = idSimilarityMap.Keys.ToList();
            
            var songs = await _db.Songs
                .Include(s => s.Album)
                .Include(s => s.SongLanguages).ThenInclude(sl => sl.Language)
                .Include(s => s.GenreMusic)
                .Include(s => s.SongArtists).ThenInclude(sa => sa.Artist).ThenInclude(a => a.Account)
                .Where(s => songIds.Contains(s.Id) && s.Active)
                .ToListAsync();
            
            var results = songs.Select(song => new SongViewWithSimilarities
        {
            Id = song.Id,
            Title = song.Title,
            ReleaseDate = song.ReleaseDate,
            
            Lyrics = song.Lyrics,

            ArtistIds = song.SongArtists.Select(sa => sa.ArtistId).ToList(),

            SimilarityPercentage = idSimilarityMap.TryGetValue(song.Id, out var sim) ? sim : 0
        })
        .OrderByDescending(dto => dto.SimilarityPercentage)
        .ToList();

        return results;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in PostChatSong: {ex.Message}");
            return new List<SongViewWithSimilarities>();
        }
    }
    
    public async Task<int[]> GetMonthlyListenCountsBySong(long songId)
    {
        var endDate = DateTime.Now;
        var startDate = endDate.AddMonths(-11); // il y a 11 mois + mois courant = 12 mois
        int[] monthlyCounts = new int[12];

        var results = await _db.Plays
            .Where(p => p.SongId == songId && p.PlayDate.Date>= startDate && p.PlayDate.Date <= endDate)
            .ToListAsync();

        foreach (var play in results)
        {
            int monthDiff = ((play.PlayDate.Year - startDate.Year) * 12) + (play.PlayDate.Month - startDate.Month);
            if (monthDiff >= 0 && monthDiff < 12)
            {
                monthlyCounts[monthDiff]++;
            }
        }

        return monthlyCounts;
    }

    public async Task<(float MedianSimilarity, List<float> SimilarityPercentages)>
        CompareSongToUserFavoritesAsync(string userId, long songIdToCompare)
    {
        const int maxLikesUsed = 20;

        // Récupérer les 20 dernières chansons likées
        var likedSongs = (await GetFavoriteSongs(userId))
            .Where(s => !string.IsNullOrEmpty(s.WavFilePath) || !string.IsNullOrEmpty(s.Mp3FilePath))
            .OrderByDescending(s => s.ReleaseDate)
            .Take(maxLikesUsed)
            .ToList();
        Console.WriteLine($"Comparing song ID {songIdToCompare} to {likedSongs.Count} liked songs...");

        // Afficher la chanson dans le terminal
        Console.WriteLine($"Comparing song ID {songIdToCompare} to {likedSongs.Count} liked songs...");
        var targetSong = await _db.Songs
            .Include(s => s.SongArtists).ThenInclude(sa => sa.Artist).ThenInclude(a => a.Account)
            .Include(s => s.Album)
            .Include(s => s.SongLanguages).ThenInclude(sl => sl.Language)
            .Include(s => s.GenreMusic)
            .Include(s => s.SongComposers).ThenInclude(sc => sc.Composer)
            .Include(s => s.SongWriters).ThenInclude(sw => sw.Writer)
            .Include(s => s.SongCrOwners).ThenInclude(sc => sc.CrOwner)
            .FirstOrDefaultAsync(s => s.Id == songIdToCompare && s.Active);
        
        if (targetSong == null || 
            (string.IsNullOrEmpty(targetSong.WavFilePath) && string.IsNullOrEmpty(targetSong.Mp3FilePath)))
            return (0f, new List<float>());
        
        var filePath = targetSong.WavFilePath ?? targetSong.Mp3FilePath;
        var fullPath = Path.Combine(_fileDirectory, filePath);
        var targetVector = await _audioFeatureExtractor.ExtractVectorAsync(fullPath);

        var similarityPercentages = new List<float>();

        foreach (var likedSong in likedSongs)
        {
            if (likedSong.Id == songIdToCompare) continue; // Éviter la comparaison avec elle-même

            var score = await _qdrantService.CompareVectorToPointAsync(targetVector, (int)likedSong.Id);
            similarityPercentages.Add(score * 100f); // en pourcentage
        }
        Console.WriteLine($"Comparing song ID {songIdToCompare} to {likedSongs.Count} liked songs...");

        Console.WriteLine($"Comparing song ID {songIdToCompare} to {likedSongs.Count} liked songs...");

        similarityPercentages.Sort();

        float median;
        int count = similarityPercentages.Count;
        if (count % 2 == 1)
            median = similarityPercentages[count / 2];
        else
            median = (similarityPercentages[(count / 2) - 1] + similarityPercentages[count / 2]) / 2;

        return (median, similarityPercentages);
    }
}