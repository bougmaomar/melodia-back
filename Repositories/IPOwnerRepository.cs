using melodia.Entities;
using melodia_api.Models.POwner;

namespace melodia_api.Repositories
{
    public interface IPOwnerRepository
    {
        public Task<POwner> FindPOwnerById(long id);
        public Task<POwner> CreatePOwner(POwnerCreateDto pOwner);
        public Task<POwner> UpdatePOwner(POwner pOwner);
        public Task<List<POwner>> GetAllPOwners();
        public Task DesactivatePOwnerById(long id);
        public Task ActivatePOwnerById(long id);
    }
}
