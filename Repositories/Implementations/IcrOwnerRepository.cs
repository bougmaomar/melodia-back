using DocumentFormat.OpenXml.Bibliography;
using melodia.Configurations;
using melodia.Entities;
using melodia_api.Exceptions;
using melodia_api.Models.SongCROwner;
using Microsoft.EntityFrameworkCore;

namespace melodia_api.Repositories.Implementations
{
    public class IcrOwnerRepository : ICROwnerRepository
    {
        public readonly MelodiaDbContext _db;
        
        public IcrOwnerRepository(MelodiaDbContext db, IConfiguration configuration) { _db = db; }

        public async Task<CROwner> CreateCROwner(CROwnerCreateDto ownerCreateDto)
        {
            CROwner owner = new CROwner()
            {
                Name = ownerCreateDto.Name,
               Active = true
            };

            _db.Owners.Add(owner);
            await _db.SaveChangesAsync();
            return owner;
        }

        public async Task<CROwner> FindCROwnerById(long id)
        {
            var owner = await _db.Owners.AsNoTracking().SingleOrDefaultAsync(o => o.Id == id);
            if (owner == null) throw new EntityNotFoundException(nameof(CROwner), nameof(id), id.ToString());
            return owner;
        }

        public Task<List<CROwner>> GetAllCROwners()
        {
            return _db.Owners.ToListAsync();
        }

        public async Task<CROwner> UpdateCROwner(CROwner owner)
        {
            if (!_db.Owners.Any(at => at.Id == owner.Id)) throw new EntityNotFoundException(nameof(CROwner), nameof(owner.Id), owner.Id.ToString());
            
            _db.Owners.Update(owner);
            await _db.SaveChangesAsync();
            return owner;
        }

        public async Task DesactivateCROwnerById(long id)
        {
            var owner = _db.Owners.SingleOrDefault(at => at.Id == id && at.Active);
            if (owner == null) throw new EntityNotFoundException(nameof(CROwner), nameof(id), id.ToString());
            owner.Active = false;
            _db.Owners.Update(owner);
            await _db.SaveChangesAsync();
        }
        public async Task ActivateCROwnerById(long id)
        {
            var owner = _db.Owners.SingleOrDefault(at => at.Id == id && !at.Active);
            if (owner == null) throw new EntityNotFoundException(nameof(CROwner), nameof(id), id.ToString());
            owner.Active = true;
            _db.Owners.Update(owner);
            await _db.SaveChangesAsync();
        }
    }
}
