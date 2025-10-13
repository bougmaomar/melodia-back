using AutoMapper;
using melodia_api.Exceptions;
using melodia.Configurations;
using melodia.Entities;
using Microsoft.EntityFrameworkCore;
using melodia_api.Models.SongComposer;

namespace melodia_api.Repositories.Implementations;

public class SongComposerRepository : ISongComposerRepository
{
    private readonly MelodiaDbContext _db;
    private readonly IMapper _mapper;

    public SongComposerRepository(MelodiaDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }
    
    public async Task<SongComposer> CreateSongComposer(SongComposerCreateDto songComposerCreateDto)
    {
        if ((await _db.Songs.FirstOrDefaultAsync(e => e.Id == songComposerCreateDto.SongId && e.Active) == null)) throw new EntityNotFoundException(nameof(Song), nameof(songComposerCreateDto.SongId), songComposerCreateDto.SongId.ToString());
        if ((await _db.Composers.FirstOrDefaultAsync(e => e.Id == songComposerCreateDto.ComposerId && e.Active) == null)) new EntityNotFoundException(nameof(Composer), nameof(songComposerCreateDto.ComposerId), songComposerCreateDto.ComposerId.ToString());
        SongComposer songComposer = new SongComposer
        {
            SongId = songComposerCreateDto.SongId,
            ComposerId = songComposerCreateDto.ComposerId,
        };
        _db.SongComposers.Add(songComposer);
        await _db.SaveChangesAsync();
        return songComposer;
    }

    public async Task<SongComposer> FindSongComposerById(long id)
    {
        var songComposer = await _db.SongComposers
            .Include(s => s.Song)
            .Include(s => s.Composer)
            .SingleOrDefaultAsync(ed => ed.Id == id && ed.Active);

        if (songComposer == null) throw new EntityNotFoundException(nameof(SongComposer), nameof(id), id.ToString());
        return songComposer;
    }
    
    
    public async Task<SongComposer> UpdateSongComposer(SongComposerUpdateDto songComposerUpdateDto)
    {
        if (!_db.SongComposers.Any(ec => ec.Id == songComposerUpdateDto.Id)) throw new EntityNotFoundException(nameof(SongComposer), nameof(SongComposer.Id), songComposerUpdateDto.Id.ToString());
        var existingSongComposer = await _db.SongComposers.FirstOrDefaultAsync(ed => ed.Id == songComposerUpdateDto.Id);
        songComposerUpdateDto.SongId = existingSongComposer.SongId;
        songComposerUpdateDto.ComposerId = existingSongComposer.ComposerId;
        _db.ChangeTracker.Clear();
        SongComposer songComposer = new SongComposer
        {
            Id = songComposerUpdateDto.Id,
            SongId = songComposerUpdateDto.SongId,
            ComposerId = songComposerUpdateDto.ComposerId,
        };
        _db.SongComposers.Update(songComposer);
        await _db.SaveChangesAsync();
        return songComposer;
    }
    
    public async Task DeactivateSongComposerById(long id)
    {
        var songComposer = await _db.SongComposers.SingleOrDefaultAsync(ed => ed.Id == id && ed.Active);
        if (songComposer == null) throw new EntityNotFoundException(nameof(SongComposer), nameof(id), id.ToString());
        songComposer.Active = false;
        _db.SongComposers.Update(songComposer);
        await _db.SaveChangesAsync();
    }
}