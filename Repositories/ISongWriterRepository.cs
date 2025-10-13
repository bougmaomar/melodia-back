using melodia.Entities;
using melodia_api.Models.SongWriter;

namespace melodia_api.Repositories;

public interface ISongWriterRepository
{
    public Task<SongWriter> CreateSongWriter(SongWriterCreateDto songWriter);
    public Task<SongWriter> FindSongWriterById(long id);

    public Task<SongWriter> UpdateSongWriter(SongWriterUpdateDto songWriter);
    public Task DeactivateSongWriterById(long id);
}