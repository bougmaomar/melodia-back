using melodia_api.Entities;

namespace melodia_api.Repositories;

public interface IAccessRepository
{
    public Task<IEnumerable<long>> GetAllValidAccessIds();
    public Task<IEnumerable<Access>> GetAllAccesses();
    public Task<Access> GetAccessById(long id);
    public Task<IEnumerable<Access>> GetAccessesByRoleId(string roleId);
    public Task AddAccess(Access access);
    public Task AddAccessToRole(string roleId, List<Section> sections);
    public Task UpdateAccess(Access access);
    public Task UpdateAccessBySectionAndRole(string roleId, long sectionId, Access access);
    public Task DeleteAccess(long id);

    public Task<Dictionary<string, object>> GetRoleAccesses(string roleId);
}