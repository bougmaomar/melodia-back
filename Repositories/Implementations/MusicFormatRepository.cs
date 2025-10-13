using melodia.Configurations;
using melodia.Entities;
using melodia_api.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace melodia_api.Repositories.Implementations
{
    public class MusicFormatRepository : IMusicFormatRepository
    {
        public readonly MelodiaDbContext _db;
        public MusicFormatRepository(MelodiaDbContext db)
        {
            _db = db;
        }
        public async Task<MusicFormat> CreateMusicFormat(MusicFormat musicFormat)
        {
            _db.MusicFormats.Add(musicFormat);
            await _db.SaveChangesAsync();
            return musicFormat;
        }
        public async Task<MusicFormat> FindMusicFormatById(long id)
        {
            var musicFormat = await _db.MusicFormats.AsNoTracking().SingleOrDefaultAsync(at => at.Id == id);
            if (musicFormat == null)
                throw new EntityNotFoundException(nameof(musicFormat), nameof(id), id.ToString());
            return musicFormat;
        }
        public Task<List<MusicFormat>> GetAllMusicFormats()
        {
            return _db.MusicFormats.Where(at => at.Active).AsNoTracking().ToListAsync();
        }
        public async Task<MusicFormat> UpdateMusicFormat(MusicFormat musicFormat)
        {
            if (!_db.MusicFormats.Any(at => at.Id == musicFormat.Id && at.Active))
                throw new EntityNotFoundException(nameof(musicFormat), nameof(musicFormat.Id), musicFormat.Id.ToString());
            _db.MusicFormats.Update(musicFormat);
            await _db.SaveChangesAsync();
            return musicFormat;
        }
        public async Task DesactivateMusicFormatById(long id)
        {
            var musicFormat = _db.MusicFormats.SingleOrDefault(at => at.Id == id && at.Active);
            if (musicFormat == null)
                throw new EntityNotFoundException(nameof(musicFormat), nameof(musicFormat.Id), musicFormat.Id.ToString());
            musicFormat.Active = false;
            _db.MusicFormats.Update(musicFormat);
            await _db.SaveChangesAsync();
        }
    }
}
