using melodia_api.Exceptions;
using melodia.Configurations;
using melodia.Entities;
using Microsoft.EntityFrameworkCore;

namespace melodia_api.Repositories.Implementations;

public class GenreMusicRepository : IGenreMusicRepository
{
    public readonly MelodiaDbContext _db;
    public GenreMusicRepository(MelodiaDbContext db) { _db = db; }
    
    public async Task<GenreMusic> CreateGenreMusic(GenreMusic genreMusic)
    {
        _db.GenreMusics.Add(genreMusic);
        await _db.SaveChangesAsync();
        return genreMusic;
    }
    
    public async Task<GenreMusic> FindGenreMusicById(long id)
    {
        var genreMusic = await _db.GenreMusics.AsNoTracking().SingleOrDefaultAsync(at => at.Id == id);
        if (genreMusic == null) throw new EntityNotFoundException(nameof(GenreMusic), nameof(id), id.ToString());
        return genreMusic;
    }
    
    public Task<List<GenreMusic>> GetAllGenreMusics()
    {
        return _db.GenreMusics.ToListAsync();
    }
    
    public async Task<GenreMusic> UpdateGenreMusic(GenreMusic genreMusic)
    {
        if (!_db.GenreMusics.Any(at => at.Id == genreMusic.Id)) throw new EntityNotFoundException(nameof(GenreMusic), nameof(genreMusic.Id), genreMusic.Id.ToString());
        _db.GenreMusics.Update(genreMusic);
        await _db.SaveChangesAsync();
        return genreMusic;
    }
    
    public async Task<bool> DesactivateGenreMusic(long genreId)
    {
        var genre = await _db.GenreMusics
            .Include(g => g.Songs)
            .FirstOrDefaultAsync(g => g.Id == genreId);
        if (genre == null) return false;

        genre.Active = false;
        foreach (var song in genre.Songs)
        {
            song.Active = false;
        }

        await _db.SaveChangesAsync();
        return true;
    }

    
    public async Task<bool> ActivateGenreMusic(long genreId)
    {
        var genre = await _db.GenreMusics
            .Include(g => g.Songs)
            .FirstOrDefaultAsync(g => g.Id == genreId);
        if (genre == null) return false;

        genre.Active = true;
        foreach (var song in genre.Songs)
        {
            song.Active = true;
        }

        await _db.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> DeleteGenreMusic(long genreId)
    {
        var genre = await _db.GenreMusics
            .Include(g => g.Songs)
            .FirstOrDefaultAsync(g => g.Id == genreId);
        if (genre == null) return false;

        _db.Songs.RemoveRange(genre.Songs);
        _db.GenreMusics.Remove(genre);

        await _db.SaveChangesAsync();
        return true;
    }
}