using System.Security.Claims;
using melodia.Entities;

namespace melodia_api.Repositories;

public interface IRoleRepository
{
    public Task<Role> GetRoleById(string roleId);
    public Task<IEnumerable<Role>> GetAllRoles();
    public Task<IEnumerable<Role>> GetAllActivatedRoles();
    public Task CreateRole(Role role);
    public Task DeleteRole(string roleId);
    public Task UpdateRole(Role role);
    public Task<bool> DeactivateRoletById(string roleId);
    public Task<bool> ActivateRoletById(string roleId);
    public Task<Role> FindRoleByName(string name);
}