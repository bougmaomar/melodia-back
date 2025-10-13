using melodia.Configurations;
using melodia.Entities;
using melodia_api.Exceptions;
using melodia_api.Models.SongComposer;
using Microsoft.EntityFrameworkCore;

namespace melodia_api.Repositories.Implementations
{
    public class ComposerRepository : IComposerRepository
    {
        public readonly MelodiaDbContext _db;

        public ComposerRepository(MelodiaDbContext db, IConfiguration configuration) { _db = db;  }

        public async Task<Composer> CreateComposer(ComposerCreateDto composerCreateDto)
        {
            Composer composer = new Composer()
            {
                Name = composerCreateDto.Name,
                Active = true
            };


            _db.Composers.Add(composer);
            await _db.SaveChangesAsync();
            return composer;
        }

        public async Task<Composer> FindComposerById(long id)
        {
            var composer = await _db.Composers.AsNoTracking().SingleOrDefaultAsync(c => c.Id == id);
            if (composer == null) throw new EntityNotFoundException(nameof(Composer), nameof(id), id.ToString());
            return composer;
        }

        public Task<List<Composer>> GetAllComposers()
        {
            return _db.Composers.ToListAsync();
        }

        public async Task<Composer> UpdateComposer(Composer composer)
        {
            if (!_db.Composers.Any(at => at.Id == composer.Id)) throw new EntityNotFoundException(nameof(Composer), nameof(composer.Id), composer.Id.ToString());
            
            _db.Composers.Update(composer);
            await _db.SaveChangesAsync();
            return composer;
        }

        public async Task DesactivateComposerById(long id)
        {
            var composer = _db.Composers.SingleOrDefault(at => at.Id == id && at.Active);
            if (composer == null) throw new EntityNotFoundException(nameof(Composer), nameof(id), id.ToString());
            composer.Active = false;
            _db.Composers.Update(composer);
            await _db.SaveChangesAsync();
        }
        public async Task ActivateComposerById(long id)
        {
            var composer = _db.Composers.SingleOrDefault(at => at.Id == id && !at.Active);
            if (composer == null) throw new EntityNotFoundException(nameof(Composer), nameof(id), id.ToString());
            composer.Active = true;
            _db.Composers.Update(composer);
            await _db.SaveChangesAsync();
        }
    }
}
