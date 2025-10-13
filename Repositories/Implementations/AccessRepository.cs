using melodia_api.Entities;
using melodia.Configurations;
using Microsoft.EntityFrameworkCore;

namespace melodia_api.Repositories.Implementations;

public class AccessRepository : IAccessRepository
{
    private readonly MelodiaDbContext _db;

    public AccessRepository(MelodiaDbContext db) { _db = db; }

    public async Task<IEnumerable<long>> GetAllValidAccessIds()
    {
        return await _db.Accesses
            .AsNoTracking().Select(acc => acc.Id)
            .ToListAsync();
    }

    public async Task<IEnumerable<Access>> GetAllAccesses()
    {
        return await _db.Accesses
            .Include(a => a.Role)
            .Include(a => a.Section)
            .AsNoTracking().ToListAsync();
    }
    
    public async Task<Access> GetAccessById(long id)
    {
        return await _db.Accesses
            .Include(a => a.Role)
            .Include(a => a.Section)
            .AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Access>> GetAccessesByRoleId(string roleId)
    {
        return await _db.Accesses
            .Include(a => a.Role)
            .Include(a => a.Section)
            .Where(a => a.RoleId == roleId)
            .AsNoTracking().ToListAsync();
    }

    public async Task AddAccess(Access access)
    {
        await _db.Accesses.AddAsync(access);
        await _db.SaveChangesAsync();
    }

    public async Task AddAccessToRole(string roleId, List<Section> sections)
    {
        foreach (var section in sections)
        {
            var access = new Access
            {
                RoleId = roleId,
                SectionId = section.Id,
                Insert = true,
                Read = true,
                Update = true,
                Delete = true
            };
            await _db.Accesses.AddAsync(access);
        }

        await _db.SaveChangesAsync();
    }

    public async Task UpdateAccess(Access access)
    {
        _db.Accesses.Update(access);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAccessBySectionAndRole(string roleId, long sectionId, Access access)
    {
        var existingAccess = await _db.Accesses
            .Where(a => a.RoleId == roleId && a.SectionId == sectionId)
            .FirstOrDefaultAsync();

        if (existingAccess != null)
        {
            existingAccess.Insert = access.Insert;
            existingAccess.Read = access.Read;
            existingAccess.Update = access.Update;
            existingAccess.Delete = access.Delete;

            _db.Accesses.Update(existingAccess);
            await _db.SaveChangesAsync();
        }
    }

    public async Task DeleteAccess(long id)
    {
        var access = await _db.Accesses.FindAsync(id);
        if (access != null)
        {
            _db.Accesses.Remove(access);
            await _db.SaveChangesAsync();
        }
    }
    
    public async Task<Dictionary<string, object>> GetRoleAccesses(string roleId) 
    {
        var role = await _db.Roles.Include(r => r.Accesses)
            .ThenInclude(a => a.Section)
            .AsNoTracking().FirstOrDefaultAsync(r => r.Id == roleId);

        if (role == null) return null;

        var accesses = new Dictionary<string, Dictionary<string, bool>>();

        foreach (var access in role.Accesses)
        {
            var sectionAccesses = new Dictionary<string, bool>
            {
                { "read", access.Read },
                { "insert", access.Insert },
                { "update", access.Update },
                { "delete", access.Delete }
            };

            accesses.Add(access.Section.Label, sectionAccesses);
        }

        return new Dictionary<string, object>
        {
            { "Role", role.Name },
            { "accesses", accesses }
        };
    }
}