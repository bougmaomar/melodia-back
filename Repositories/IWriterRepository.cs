

using melodia.Entities;
using melodia_api.Models.SongWriter;

namespace melodia_api.Repositories
{
    public interface IWriterRepository
    {
        public Task<Writer> FindWriterById(long id);
        public Task<Writer> CreateWriter(WriterCreateDto writer);
        public Task<Writer> UpdateWriter(Writer writer);
        public Task<List<Writer>> GetAllWriters();
        public Task DesactivateWriterById(long id);
        public Task ActivateWriterById(long id);
    }

}
