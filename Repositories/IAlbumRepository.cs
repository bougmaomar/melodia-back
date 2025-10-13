using melodia.Entities;
using melodia_api.Models.Album;

namespace melodia_api.Repositories
{
    public interface IAlbumRepository
    {
        public Task<Album> CreateAlbum(AlbumCreateDto albumCreateDto);

        public Task<List<Album>> FilterAlbums(string title = null, TimeSpan? minTotalDuration = null,
            TimeSpan? maxTotalDuration = null, long? albumTypeId = null);

        public Task<Album> UpdateAlbum(Album updatedAlbum, IFormFile newCoverImage);
        public Task DeactivateAlbumById(long id);
        public Task ActivateAlbumById(long id);
        public Task<List<Album>> GetAllAlbums();
        public Task<List<AlbumType>> GetAlbumTypes();
        public Task<List<Album>> GetAlbumsByTypes(long typeId);
        public Task<Album> FindAlbumById(long id);
        public Task<List<Album>> GetAlbumByArtist(long artistId);
        public Task<List<Album>> GetRelatedAlbums(long albumId);
        public Task<Album> GetAlbumById(long id);

    }
}
