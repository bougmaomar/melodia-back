using melodia_api.Models.Song;
using melodia.Entities;
using melodia_api.Entities;

namespace melodia_api.Repositories;

public interface ISongRepository
{
    public Task<Song> CreateSong(SongCreateDto songCreateDto);
    public Task<Song> UpdateSong(Song updatedSong, IFormFile newAudioFile = null, IFormFile newWavFile = null,
        IFormFile newCoverImage = null);

    public Task<Song> UpdateSongSocialMedia(long songId, string mediaType, string media);
    public Task<int> GetSongPlays(long songId);
    public Task<int> GetSongDownloads(long songId);
    public Task DeactivateSongById(long id);
    public Task ActivateSongById(long id);
    public Task<List<Song>> GetAllSongs();
    public Task<List<Song>> GetSongsForStation();
    public Task<Song> FindSongById(long id);
    public Task<Song> GetSongById(long id);
    public Task<List<Song>> FindSongsByLanguage(long languageId);
    public Task<List<Song>> FindSongsByType(long typeId);
    public Task<List<Song>> FindSongsByArtist(long artistId);
    public Task<List<Song>> FindSongByArtistEmail(string artistEmail);
    public Task RecordDownloadAsync(long songId, long radioStationId);
    public Task RecordPlayAsync(long songId, long radioStationId);
    public Task RecordSongVisits(long songId, long radioStationId);
    public Task<int> GetSongVisits(long songId);
    public Task<int> GetSuggestNumbers(long songId);
    public Task<int> GetSongStatisticsAsync();
    public Task<List<Download>> GetAllDownloadsByArtist(long artistId);
    public Task<List<Play>> GetAllPlaysByArtist(long artistId);
    public Task<List<Song>> GetRelatedSongs(long songId);
    public Task<List<Song>> GetRecommendedSongsAsync(long radioStationId);
    public Task<List<Song>> GetTrendingSongsAsync();
    public Task<List<Song>> GetLatestAddedSongsAsync();
    public Task<List<Song>> GetMostDownloadedSongsAsync();
    public Task<List<Song>> GetMostPlayedSongsAsync();
    public Task<List<Song>> GetFavoriteSongs(string userId);
    public Task<bool> AddFavoriteSong(string userId, long songId);
    public Task<bool> RemoveFavoriteSong(string userId, long songId);
    public Task<bool> IsFavoriteSong(string userId, long songId);
    public Task<List<KeyValuePair<int, int>>> GetAllSongsByDecade();
    public Task<List<KeyValuePair<int, int>>> GetAllSongsByDuration();
    public Task<List<KeyValuePair<int, int>>> GetAllAcceptsByDecade();
    public Task<List<KeyValuePair<int, int>>> GetAllAcceptsByDuration();
    public Task<List<KeyValuePair<string, int>>> GetMostUsedTypes();
    public Task<List<KeyValuePair<string, int>>> GetMostUsedLanguages();
    public Task<long> GetPlaysCountBySongId(long songId);
    public Task<long> GetDownloadsCountBySongId(long songId);
    public Task<int[]> GetMensualSongsByArtist(long artistId);
    public Task<int[]> GetAnnualSongsByArtist(long artistId);
    public Task<List<Song>> SearchSongs(string searchTerm);
    public Task<List<(int, float, string)>> SearchSongsByAudio(string audioFilePath, int limit);

    public Task<List<Song>> GetRecommendedSongsByFavoritesAsync(string userId, int limit);

    public Task<List<SongViewWithSimilarities>> PostChatSong(string message);

    public Task<int[]> GetMonthlyListenCountsBySong(long songId);

    public Task<(float MedianSimilarity, List<float> SimilarityPercentages)>
        CompareSongToUserFavoritesAsync(string userId, long songIdToCompare);
}
