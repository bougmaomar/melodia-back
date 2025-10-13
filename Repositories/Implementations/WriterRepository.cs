using melodia.Configurations;
using melodia.Entities;
using melodia_api.Exceptions;
using melodia_api.Models.SongWriter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace melodia_api.Repositories.Implementations
{
    public class WriterRepository : IWriterRepository
    {
        public readonly MelodiaDbContext _db;

        public WriterRepository(MelodiaDbContext db, IConfiguration configuration) { _db = db; }

        public async Task<Writer> CreateWriter(WriterCreateDto writerCreateDto)
        {
            Writer writer = new Writer()
            {
                Name = writerCreateDto.Name,
                Active = true
            };

            _db.Writers.Add(writer);
            await _db.SaveChangesAsync();
            return writer;
        }

        public async Task<Writer> FindWriterById(long id)
        {
            var writer = await _db.Writers.AsNoTracking().SingleOrDefaultAsync(w => w.Id == id);
            if (writer == null) throw new EntityNotFoundException(nameof(Writer), nameof(id), id.ToString());
            return writer;
        }

        public Task<List<Writer>> GetAllWriters()
        {
            return _db.Writers.ToListAsync();
        }

        public async Task<Writer> UpdateWriter(Writer writer)
        {
            if (!_db.Writers.Any(at => at.Id == writer.Id)) throw new EntityNotFoundException(nameof(Writer), nameof(writer.Id), writer.Id.ToString());
           
            _db.Writers.Update(writer);
            await _db.SaveChangesAsync();
            return writer;
        }

        public async Task DesactivateWriterById(long id)
        {
            var writer = _db.Writers.SingleOrDefault(at => at.Id == id && at.Active);
            if (writer == null) throw new EntityNotFoundException(nameof(Writer), nameof(id), id.ToString());
            writer.Active = false;
            _db.Writers.Update(writer);
            await _db.SaveChangesAsync();
        }
        
        public async Task ActivateWriterById(long id)
        {
            var writer = _db.Writers.SingleOrDefault(at => at.Id == id && !at.Active);
            if (writer == null) throw new EntityNotFoundException(nameof(Writer), nameof(id), id.ToString());
            writer.Active = true;
            _db.Writers.Update(writer);
            await _db.SaveChangesAsync();
        }
    }
}
