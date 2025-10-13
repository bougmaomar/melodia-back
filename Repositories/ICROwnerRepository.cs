using melodia.Entities;
using melodia_api.Models.SongCROwner;

namespace melodia_api.Repositories
{
    public interface ICROwnerRepository
    {
        public Task<CROwner> FindCROwnerById(long id);
        public Task<CROwner> CreateCROwner(CROwnerCreateDto crOwner);
        public Task<CROwner> UpdateCROwner(CROwner crOwner);
        public Task<List<CROwner>> GetAllCROwners();
        public Task DesactivateCROwnerById(long id);
        public Task ActivateCROwnerById(long id);
    }
}
