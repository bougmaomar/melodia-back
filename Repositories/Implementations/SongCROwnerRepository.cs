using AutoMapper;
using melodia_api.Exceptions;
using melodia.Configurations;
using melodia.Entities;
using Microsoft.EntityFrameworkCore;
using melodia_api.Models.SongCROwner;

namespace melodia_api.Repositories.Implementations;

public class SongCROwnerRepository : ISongCROwnerRepository
{
    private readonly MelodiaDbContext _db;
    private readonly IMapper _mapper;

    public SongCROwnerRepository(MelodiaDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }
    
    public async Task<SongCROwner> CreateSongCROwner(SongCROwnerCreateDto songCROwnerCreateDto)
    {
        if (await _db.Songs.FirstOrDefaultAsync(e => e.Id == songCROwnerCreateDto.SongId && e.Active) == null) throw new EntityNotFoundException(nameof(Song), nameof(songCROwnerCreateDto.SongId), songCROwnerCreateDto.SongId.ToString());
        SongCROwner songCROwner = new SongCROwner
        {
            SongId = songCROwnerCreateDto.SongId,
            CROwnerId = songCROwnerCreateDto.CROwnerId
        };
        _db.SongCrOwners.Add(songCROwner);
        await _db.SaveChangesAsync();
        return songCROwner;
    }

    public async Task<SongCROwner> FindSongCROwnerById(long id)
    {
        var songCROwner = await _db.SongCrOwners
            .Include(s => s.CrOwner)
            .Include(s => s.Song)
            .SingleOrDefaultAsync(ed => ed.Id == id && ed.Active);
        if (songCROwner == null) throw new EntityNotFoundException(nameof(SongCROwner), nameof(id), id.ToString());
        return songCROwner;
    }
    
    
    public async Task<SongCROwner> UpdateSongCROwner(SongCROwnerUpdateDto songCROwnerUpdateDto)
    {
        if (!_db.SongCrOwners.Any(ec => ec.Id == songCROwnerUpdateDto.Id)) throw new EntityNotFoundException(nameof(SongCROwner), nameof(SongCROwner.Id), songCROwnerUpdateDto.Id.ToString());
        var existingSongCROwner = await _db.SongCrOwners.FirstOrDefaultAsync(ed => ed.Id == songCROwnerUpdateDto.Id);
        songCROwnerUpdateDto.SongId = existingSongCROwner.SongId;
        songCROwnerUpdateDto.CROwnerId = existingSongCROwner.CROwnerId;
        _db.ChangeTracker.Clear();
        SongCROwner songCROwner = new SongCROwner
        {
            SongId = songCROwnerUpdateDto.SongId,
            CROwnerId = songCROwnerUpdateDto.CROwnerId
        };
        _db.SongCrOwners.Update(songCROwner);
        await _db.SaveChangesAsync();
        return songCROwner;
    }
    
    public async Task DeactivateSongCROwnerById(long id)
    {
        var songCROwner = await _db.SongCrOwners.SingleOrDefaultAsync(ed => ed.Id == id && ed.Active);
        if (songCROwner == null) throw new EntityNotFoundException(nameof(SongCROwner), nameof(id), id.ToString());
        songCROwner.Active = false;
        _db.SongCrOwners.Update(songCROwner);
        await _db.SaveChangesAsync();
    }
}