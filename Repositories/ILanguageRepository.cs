using melodia.Entities;

namespace melodia_api.Repositories
{
    public interface ILanguageRepository
    {

        public Task<Language> FindLanguageById(long id);
        public Task<Language> CreateLanguage(Language language);
        public Task<Language> UpdateLanguage(Language language);
        public Task<List<Language>> GetAllLanguages();
        public Task<bool> DesactivateLanguage(long languageId);
        public Task<bool> ActivateLanguage(long languageId);
        public Task<bool> DeleteLanguage(long genreId);
    }
}
