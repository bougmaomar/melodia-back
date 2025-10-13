using melodia_api.Exceptions;
using melodia.Configurations;
using melodia.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace melodia_api.Repositories.Implementations;

public class RoleRepository : IRoleRepository
{
    private readonly RoleManager<Role> _roleManager;
    private readonly MelodiaDbContext _db;

    public RoleRepository(RoleManager<Role> roleManager, MelodiaDbContext db)
    {
        _roleManager = roleManager; _db = db;
    }

    public async Task<IEnumerable<Role>> GetAllRoles()
    {
        return await _roleManager.Roles.ToListAsync();
    }
    
    public async Task<IEnumerable<Role>> GetAllActivatedRoles()
    {
        return await _roleManager.Roles.Where(r => r.Active).ToListAsync();
    }
    
    public async Task CreateRole(Role role)
    {
        var existingRole = await _db.Roles.FirstOrDefaultAsync(r => r.Name == role.Name);

        if (existingRole != null) throw new Exception($"Role name '{role.Name}' is already taken.");
        
        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            throw new Exception($"Failed to create role: {string.Join(", ", errors)}");
        }
    }

    public async Task DeleteRole(string roleId)
    {
        var role = await GetRoleById(roleId);
        if (role != null)
        {
            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded) throw new Exception("Failed to delete role");
        }
    }
   
    public async Task UpdateRole(Role role)
    {
        var existingRole = await _roleManager.FindByIdAsync(role.Id);
        existingRole.Name = role.Name;
        
        var result = await _roleManager.UpdateAsync(existingRole);

        if (!result.Succeeded) throw new Exception("Failed to update role");
    }

    public async Task<bool> DeactivateRoletById(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role != null)
        {
            role.Active = false;
            var result = await _roleManager.UpdateAsync(role);
            return result.Succeeded;
        }
        return false;
    }

    public async Task<bool> ActivateRoletById(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role != null)
        {
            role.Active = true;
            var result = await _roleManager.UpdateAsync(role);
            return result.Succeeded;
        }
        return false;
    }

    public async Task<Role> FindRoleByName(string name)
    {
        return await _roleManager.FindByNameAsync(name);
    }

    public async Task<Role> GetRoleById(string roleId)
    {
        var role = await _roleManager.Roles.SingleOrDefaultAsync(role => role.Id == roleId && role.Active);
        if (role == null) throw new EntityNotFoundException(nameof(Role), nameof(roleId), roleId);
        return role;
    }
}