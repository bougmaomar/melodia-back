using melodia.Entities;
using melodia_api.Models.SongComposer;

namespace melodia_api.Repositories
{
    public interface IComposerRepository
    {
        public Task<Composer> FindComposerById(long id);
        public Task<Composer> CreateComposer(ComposerCreateDto composer);
        public Task<Composer> UpdateComposer(Composer composer);
        public Task<List<Composer>> GetAllComposers();
        public Task DesactivateComposerById(long id);
        public Task ActivateComposerById(long id);
    }
}
