using melodia.Configurations;
using melodia.Entities;
using melodia_api.Exceptions;
using melodia_api.Models.POwner;
using Microsoft.EntityFrameworkCore;

namespace melodia_api.Repositories.Implementations
{
    public class POwnerRepository : IPOwnerRepository
    {
        public readonly MelodiaDbContext _db;
        public POwnerRepository(MelodiaDbContext db, IConfiguration configuration) { _db = db; }

        public async Task<POwner> CreatePOwner(POwnerCreateDto ownerCreateDto)
        {
            POwner owner = new POwner()
            {
                Name = ownerCreateDto.Name,
                Active = true
            };

            _db.POwners.Add(owner);
            await _db.SaveChangesAsync();
            return owner;
        }

        public async Task<POwner> FindPOwnerById(long id)
        {
            var owner = await _db.POwners.AsNoTracking().SingleOrDefaultAsync(o => o.Id == id);
            if (owner == null) throw new EntityNotFoundException(nameof(POwner), nameof(id), id.ToString());
            return owner;
        }

        public Task<List<POwner>> GetAllPOwners()
        {
            return _db.POwners.ToListAsync();
        }

        public async Task<POwner> UpdatePOwner(POwner owner)
        {
            if (!_db.POwners.Any(at => at.Id == owner.Id)) throw new EntityNotFoundException(nameof(POwner), nameof(owner.Id), owner.Id.ToString());
            
            _db.POwners.Update(owner);
            await _db.SaveChangesAsync();
            return owner;
        }

        public async Task DesactivatePOwnerById(long id)
        {
            var owner = _db.POwners.SingleOrDefault(at => at.Id == id && at.Active);
            if (owner == null) throw new EntityNotFoundException(nameof(POwner), nameof(id), id.ToString());
            owner.Active = false;
            _db.POwners.Update(owner);
            await _db.SaveChangesAsync();
        }
        public async Task ActivatePOwnerById(long id)
        {
            var owner = _db.POwners.SingleOrDefault(at => at.Id == id && !at.Active);
            if (owner == null) throw new EntityNotFoundException(nameof(POwner), nameof(id), id.ToString());
            owner.Active = true;
            _db.POwners.Update(owner);
            await _db.SaveChangesAsync();
        }
    }
}

