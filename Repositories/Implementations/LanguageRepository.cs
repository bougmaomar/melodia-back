using melodia.Configurations;
using melodia.Entities;
using melodia_api.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace melodia_api.Repositories.Implementations
{
    public class LanguageRepository : ILanguageRepository
    {
        public readonly MelodiaDbContext _db;
        
        public LanguageRepository(MelodiaDbContext db) { _db = db; }
        
        public async Task<Language> CreateLanguage(Language language)
        {
            _db.Languages.Add(language);
            await _db.SaveChangesAsync();
            return language;
        }
        
        public async Task<Language> FindLanguageById(long id)
        {
            var language = await _db.Languages.AsNoTracking().SingleOrDefaultAsync(at => at.Id == id && at.Active);
            if (language == null) throw new EntityNotFoundException(nameof(Language), nameof(id), id.ToString());
            return language;
        }
        
        public Task<List<Language>> GetAllLanguages()
        {
            return _db.Languages.ToListAsync();
        }
        
        public async Task<Language> UpdateLanguage(Language language)
        {
            if (!_db.Languages.Any(at => at.Id == language.Id )) throw new EntityNotFoundException(nameof(Language), nameof(language.Id), language.Id.ToString());
            _db.Languages.Update(language);
            await _db.SaveChangesAsync();
            return language;
        }
        
        public async Task<bool> DesactivateLanguage(long languageId)
        {
            var language = await _db.Languages
                .Include(g => g.Songs)
                .FirstOrDefaultAsync(g => g.Id == languageId);
            if (language == null) return false;

            language.Active = false;
            foreach (var song in language.Songs)
            {
                song.Active = false;
            }

            await _db.SaveChangesAsync();
            return true;
        }

    
        public async Task<bool> ActivateLanguage(long languageId)
        {
            var language = await _db.Languages
                .Include(g => g.Songs)
                .FirstOrDefaultAsync(g => g.Id == languageId);
            if (language == null) return false;

            language.Active = true;
            foreach (var song in language.Songs)
            {
                song.Active = true;
            }

            await _db.SaveChangesAsync();
            return true;
        }
        
        public async Task<bool> DeleteLanguage(long languageId)
        {
            var language = await _db.Languages
                .Include(g => g.Songs)
                .FirstOrDefaultAsync(g => g.Id == languageId);
            if (language == null) return false;

            _db.Songs.RemoveRange(language.Songs);
            _db.Languages.Remove(language);

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
