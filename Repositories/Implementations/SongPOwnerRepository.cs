using AutoMapper;
using melodia.Configurations;
using melodia.Entities;
using melodia_api.Exceptions;
using melodia_api.Models.SongPOwner;
using Microsoft.EntityFrameworkCore;

namespace melodia_api.Repositories.Implementations
{
    public class SongPOwnerRepository : ISongPOwnerRepository
    {
        private readonly MelodiaDbContext _db;
        private readonly IMapper _mapper;

        public SongPOwnerRepository(MelodiaDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

        public async Task<SongPOwner> CreateSongPOwner(SongPOwnerCreateDto songPOwnerCreateDto)
        {
            if (await _db.Songs.FirstOrDefaultAsync(e => e.Id == songPOwnerCreateDto.SongId && e.Active) == null) throw new EntityNotFoundException(nameof(Song), nameof(songPOwnerCreateDto.SongId), songPOwnerCreateDto.SongId.ToString());
            SongPOwner songPOwner = new SongPOwner
            {
                SongId = songPOwnerCreateDto.SongId,
                POwnerId = songPOwnerCreateDto.POwnerId
            };
            _db.SongPOwners.Add(songPOwner);
            await _db.SaveChangesAsync();
            return songPOwner;
        }

        public async Task<SongPOwner> FindSongPOwnerById(long id)
        {
            var songPOwner = await _db.SongPOwners
                .Include(s => s.POwner)
                .Include(s => s.Song)
                .SingleOrDefaultAsync(ed => ed.Id == id && ed.Active);
            if (songPOwner == null) throw new EntityNotFoundException(nameof(SongPOwner), nameof(id), id.ToString());
            return songPOwner;
        }


        public async Task<SongPOwner> UpdateSongPOwner(SongPOwnerUpdateDto songPOwnerUpdateDto)
        {
            if (!_db.SongPOwners.Any(ec => ec.Id == songPOwnerUpdateDto.Id)) throw new EntityNotFoundException(nameof(SongPOwner), nameof(SongPOwner.Id), songPOwnerUpdateDto.Id.ToString());
            var existingSongPOwner = await _db.SongPOwners.FirstOrDefaultAsync(ed => ed.Id == songPOwnerUpdateDto.Id);
            songPOwnerUpdateDto.SongId = existingSongPOwner.SongId;
            songPOwnerUpdateDto.POwnerId = existingSongPOwner.POwnerId;
            _db.ChangeTracker.Clear();
            SongPOwner songPOwner = new SongPOwner
            {
                SongId = songPOwnerUpdateDto.SongId,
                POwnerId = songPOwnerUpdateDto.POwnerId
            };
            _db.SongPOwners.Update(songPOwner);
            await _db.SaveChangesAsync();
            return songPOwner;
        }

        public async Task DeactivateSongPOwnerById(long id)
        {
            var songPOwner = await _db.SongPOwners.SingleOrDefaultAsync(ed => ed.Id == id && ed.Active);
            if (songPOwner == null) throw new EntityNotFoundException(nameof(SongPOwner), nameof(id), id.ToString());
            songPOwner.Active = false;
            _db.SongPOwners.Update(songPOwner);
            await _db.SaveChangesAsync();
        }
    }
}
