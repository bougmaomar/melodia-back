using AutoMapper;
using melodia_api.Models.Role;
using melodia_api.Repositories;
using melodia.Entities;
using Microsoft.AspNetCore.Mvc;

namespace melodia_api.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class RoleController : ControllerBase
{
	private readonly IMapper _mapper;
	private readonly IRoleRepository _repository;


	public RoleController(IMapper mapper, IRoleRepository repository)
	{
		_mapper = mapper;
		_repository = repository;
	}

	[HttpGet("{roleId}")]
	public async Task<IActionResult> GetRoleById(string roleId)
	{
		var role = await _repository.GetRoleById(roleId);
		return Ok(_mapper.Map<RoleViewDto>(role));
	}

	[HttpGet("all")]
	public async Task<IActionResult> GetAllRoles()
	{
		var roles = await _repository.GetAllRoles();
		return Ok(_mapper.Map<IEnumerable<RoleViewDto>>(roles));
	}

	[HttpGet("all/activated")]
	public async Task<IActionResult> GetAllActivatedRoles()
	{
		var roles = await _repository.GetAllActivatedRoles();
		var allActivatedRoles = _mapper.Map<IEnumerable<RoleViewDto>>(roles);
		return Ok(allActivatedRoles);
	}

	[HttpPost]
	public async Task<IActionResult> CreateRole(RoleCreateDto roleCreateDto)
	{
		if (roleCreateDto == null) return BadRequest("Invalid role data.");

		var existingRole = await _repository.FindRoleByName(roleCreateDto.Name);
		if (existingRole != null) return BadRequest("Role with the same name already exists.");

		try
		{
			var role = _mapper.Map<Role>(roleCreateDto);
			if (role == null)
			{
				return BadRequest("Role not found.");
			}

			await _repository.CreateRole(role);
			return Ok(_mapper.Map<RoleViewDto>(role));
		}
		catch (Exception ex)
		{
			return StatusCode(500, $"Internal server error: {ex.Message}");
		}
	}

	[HttpDelete("{roleId}")]
	public async Task<IActionResult> DeleteRole(string roleId)
	{
		await _repository.DeleteRole(roleId);
		return NoContent();
	}

	[HttpPut]
	public async Task<IActionResult> UpdateRole(RoleUpdateDto roleUpdateDto)
	{
		var role = _mapper.Map<Role>(roleUpdateDto);
		await _repository.UpdateRole(role);
		return NoContent();
	}

	[HttpPut("deactivate/{roleId}")]
	public async Task<IActionResult> DeactivateRoleById(string roleId)
	{
		var result = await _repository.DeactivateRoletById(roleId);
		if (result) { return NoContent(); }
		return NotFound();
	}

	[HttpPut("activate/{roleId}")]
	public async Task<IActionResult> ActivateIntegratorAccountById(string roleId)
	{
		var result = await _repository.ActivateRoletById(roleId);
		if (result) { return NoContent(); }
		return NotFound();
	}
}