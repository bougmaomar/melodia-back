using melodia.Entities;
using melodia_api.Models.SongComposer;

namespace melodia_api.Repositories;

public interface ISongComposerRepository
{
    public Task<SongComposer> CreateSongComposer(SongComposerCreateDto songComposer);
    public Task<SongComposer> FindSongComposerById(long id);
    public Task<SongComposer> UpdateSongComposer(SongComposerUpdateDto songComposer);
    public Task DeactivateSongComposerById(long id);
}