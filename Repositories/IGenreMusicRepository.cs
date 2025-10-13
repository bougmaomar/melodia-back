using melodia.Entities;

namespace melodia_api.Repositories;

public interface IGenreMusicRepository
{
    public Task<GenreMusic> FindGenreMusicById(long id);
    public Task<GenreMusic> CreateGenreMusic(GenreMusic genreMusic);
    public Task<GenreMusic> UpdateGenreMusic(GenreMusic genreMusic);
    public Task<List<GenreMusic>> GetAllGenreMusics();
    public Task<bool> DesactivateGenreMusic(long genreId);
    public Task<bool> ActivateGenreMusic(long genreId);
    public Task<bool> DeleteGenreMusic(long genreId);
}