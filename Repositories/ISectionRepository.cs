using melodia_api.Entities;

namespace melodia_api.Repositories;

public interface ISectionRepository
{
    bool ExistsByLabel(string label);
    void Add(Section section);
    public Task<IEnumerable<Section>> GetAllSections();
    Task<IEnumerable<Section>> GetAllSectionsByRole(string roleId);
    public Task<Section> GetSectionById(long? id);
    public Task<IEnumerable<Section>> GetSubSections(long parentId);
    public Task AddSection(Section section);
    public Task UpdateSection(Section section);
    public Task DeleteSection(long id);
    public Task SaveChangesAsync();
    public Task<IEnumerable<Section>> GetParentSections();
    public Task<IEnumerable<Section>> GetSubSectionsByParentId(long parentId);
    public Task<(List<Section> ParentSections, List<Section> SubSections)> GetSectionsByRole(string roleId);
}