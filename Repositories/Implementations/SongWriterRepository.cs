using AutoMapper;
using melodia_api.Exceptions;
using melodia.Configurations;
using melodia.Entities;
using Microsoft.EntityFrameworkCore;
using melodia_api.Models.SongWriter;
using melodia_api.Models.SongCROwner;

namespace melodia_api.Repositories.Implementations;

public class SongWriterRepository : ISongWriterRepository
{
    private readonly MelodiaDbContext _db;
    private readonly IMapper _mapper;

    public SongWriterRepository(MelodiaDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }
    
    public async Task<SongWriter> CreateSongWriter(SongWriterCreateDto songWriterCreateDto)
    {
        if (await _db.Songs.FirstOrDefaultAsync(e => e.Id == songWriterCreateDto.SongId && e.Active) == null) throw new EntityNotFoundException(nameof(Song), nameof(songWriterCreateDto.SongId), songWriterCreateDto.SongId.ToString());
        SongWriter songWriter = new SongWriter
        {
            SongId = songWriterCreateDto.SongId,
            WriterId = songWriterCreateDto.WritterId
        };
        _db.SongWriters.Add(songWriter);
        await _db.SaveChangesAsync();
        return songWriter;
    }

    public async Task<SongWriter> FindSongWriterById(long id)
    {
        var songWriter = await _db.SongWriters
            .Include(s => s.Writer)
            .Include(s => s.Song)
            .SingleOrDefaultAsync(ed => ed.Id == id && ed.Active);
        if (songWriter == null) throw new EntityNotFoundException(nameof(SongWriter), nameof(id), id.ToString());
        return songWriter;
    }
    
    
    public async Task<SongWriter> UpdateSongWriter(SongWriterUpdateDto songWriterUpdateDto)
    {
        if (!_db.SongWriters.Any(ec => ec.Id == songWriterUpdateDto.Id)) throw new EntityNotFoundException(nameof(SongWriter), nameof(SongWriter.Id), songWriterUpdateDto.Id.ToString());
        var existingSongWriter = await _db.SongWriters.FirstOrDefaultAsync(ed => ed.Id == songWriterUpdateDto.Id);
        songWriterUpdateDto.SongId = existingSongWriter.SongId;
        songWriterUpdateDto.WritterId = existingSongWriter.WriterId;
        _db.ChangeTracker.Clear();
        SongWriter songWriter = new SongWriter
        {
            SongId = songWriterUpdateDto.SongId,
            WriterId = songWriterUpdateDto.WritterId
        };
        _db.SongWriters.Update(songWriter);
        await _db.SaveChangesAsync();
        return songWriter;
    }
    
    public async Task DeactivateSongWriterById(long id)
    {
        var songWriter = await _db.SongWriters.SingleOrDefaultAsync(ed => ed.Id == id && ed.Active);
        if (songWriter == null) throw new EntityNotFoundException(nameof(SongWriter), nameof(id), id.ToString());
        songWriter.Active = false;
        _db.SongWriters.Update(songWriter);
        await _db.SaveChangesAsync();
    }
}