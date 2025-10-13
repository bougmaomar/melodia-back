using melodia.Entities;
using melodia_api.Models.SongCROwner;

namespace melodia_api.Repositories;

public interface ISongCROwnerRepository
{
    public Task<SongCROwner> CreateSongCROwner(SongCROwnerCreateDto songCrOwner);
    public Task<SongCROwner> FindSongCROwnerById(long id);
    public Task<SongCROwner> UpdateSongCROwner(SongCROwnerUpdateDto songCrOwner);
    public Task DeactivateSongCROwnerById(long id);
}