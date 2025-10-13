using melodia.Entities;
using melodia_api.Models.SongPOwner;

namespace melodia_api.Repositories
{
    public interface ISongPOwnerRepository
    {
        public Task<SongPOwner> CreateSongPOwner(SongPOwnerCreateDto songPOwner);
        public Task<SongPOwner> FindSongPOwnerById(long id);
        public Task<SongPOwner> UpdateSongPOwner(SongPOwnerUpdateDto songPOwner);
        public Task DeactivateSongPOwnerById(long id);
    }
}
