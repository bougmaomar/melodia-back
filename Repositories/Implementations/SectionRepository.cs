using melodia_api.Entities;
using melodia.Configurations;
using melodia.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace melodia_api.Repositories.Implementations;

public class SectionRepository : ISectionRepository
{
    private readonly MelodiaDbContext _db;
    private readonly RoleManager<Role> _roleManager;

    public SectionRepository(MelodiaDbContext db, RoleManager<Role> roleManager)
    {
        _db = db;
        _roleManager = roleManager;
    }
    
    public bool ExistsByLabel(string label)
    {
        return _db.Sections.Any(s => s.Label == label);
    }

    public async Task<IEnumerable<Section>> GetAllSectionsByRole(string roleId)
    {

        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null) throw new KeyNotFoundException($"Role with ID {roleId} not found.");
        var hasFullAccess = role.Name == "admin";
        
        var query = _db.Sections.Include(s => s.ParentSection)
            .Include(s => s.SubSections)
            .Where(s => s.ParentSectionId == null);
        
        if (hasFullAccess) return await query.ToListAsync();
        
        return await query.ToListAsync();
    }

    public void Add(Section section)
    {
        _db.Sections.Add(section);
    }
    
     public async Task<IEnumerable<Section>> GetAllSections()
    {
        return await _db.Sections.Include(s => s.ParentSection)
            .Include(s => s.SubSections)
            .Where(s => s.ParentSectionId == null)
            .ToListAsync();
    }

    public async Task<Section> GetSectionById(long? id)
    {
        return await _db.Sections.Include(s => s.ParentSection)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Section>> GetSubSections(long parentId)
    {
        var parentSection = await _db.Sections.FindAsync(parentId);
        if (parentSection == null || parentSection.ParentSectionId != null) return new List<Section>();

        return await _db.Sections.Include(s => s.ParentSection)
            .Where(s => s.ParentSectionId == parentId)
            .ToListAsync();
    }

    public async Task AddSection(Section section)
    {
        await _db.Sections.AddAsync(section);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateSection(Section section)
    {
        _db.Sections.Update(section);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteSection(long id)
    {
        var section = await _db.Sections.Include(s => s.SubSections).FirstOrDefaultAsync(s => s.Id == id);
        if (section != null)
        {
            if (section.SubSections != null && section.SubSections.Count > 0)
                _db.Sections.RemoveRange(section.SubSections);

            _db.Sections.Remove(section);
            await _db.SaveChangesAsync();
        }
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<Section>> GetParentSections()
    {
        return await _db.Sections.Where(s => s.ParentSectionId == null).ToListAsync();
    }

    public async Task<IEnumerable<Section>> GetSubSectionsByParentId(long parentId)
    {
        return await _db.Sections.Where(s => s.ParentSectionId == parentId).ToListAsync();
    }

    public async Task<(List<Section> ParentSections, List<Section> SubSections)> GetSectionsByRole(string roleId)
    {
        var role = await _db.Roles.Include(r => r.Accesses)
            .ThenInclude(a => a.Section)
            .FirstOrDefaultAsync(r => r.Id == roleId); 
        
        if (role == null) return (null, null);

        var parentSections = await GetParentSections();
        var allowedParentSections =
            parentSections.Where(ps => role.Accesses.Any(a => a.SectionId == ps.Id && a.Read)).ToList();
        var allowedSubSections = new List<Section>();

        foreach (var parentSection in allowedParentSections)
        {
            var subSections = await GetSubSectionsByParentId(parentSection.Id);
            var allowedSubs = subSections.Where(ss => role.Accesses.Any(a => a.SectionId == ss.Id && a.Read)).ToList();
            allowedSubSections.AddRange(allowedSubs);
        }

        return (allowedParentSections, allowedSubSections);
    }

}