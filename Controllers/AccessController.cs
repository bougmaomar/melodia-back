using AutoMapper;
using melodia_api.Entities;
using melodia_api.Models.Access;
using melodia_api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace melodia_api.Controllers;

[Route("api/accesses")]
[ApiController]
public class AccessController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IAccessRepository _repository;

    public AccessController(IAccessRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    
     [HttpGet("valid_ids")]
    public async Task<ActionResult<IEnumerable<long>>> GetValidAccessIds()
    {
        var validIds = await _repository.GetAllValidAccessIds();
        return Ok(validIds);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AccessViewDto>>> GetAllAccesses()
    {
        var accesses = await _repository.GetAllAccesses();
        return Ok(_mapper.Map<IEnumerable<AccessViewDto>>(accesses));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AccessViewDto>> GetAccessById(long id)
    {
        var access = await _repository.GetAccessById(id);
        if (access == null) return NotFound();
        return Ok(_mapper.Map<AccessViewDto>(access));
    }

    [HttpGet("role/{roleId}")]
    public async Task<ActionResult<IEnumerable<AccessViewDto>>> GetAccessesByRoleId(string roleId)
    {
        var accesses = await _repository.GetAccessesByRoleId(roleId);
        return Ok(_mapper.Map<IEnumerable<AccessViewDto>>(accesses));
    }

    [HttpPost]
    public async Task<ActionResult<AccessViewDto>> CreateAccess(AccessCreateDto accessCreateDto)
    {
        var access = _mapper.Map<Access>(accessCreateDto);
        await _repository.AddAccess(access);
        var accessViewDto = _mapper.Map<AccessViewDto>(access);
        return CreatedAtAction(nameof(GetAccessById), new { id = accessViewDto.Id }, accessViewDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAccess(long id, AccessUpdateDto accessUpdateDto)
    {
        var access = await _repository.GetAccessById(id);
        if (access == null) return NotFound();
        _mapper.Map(accessUpdateDto, access);
        await _repository.UpdateAccess(access);
        return NoContent();
    }

    [HttpPut("role/{roleId}/section/{sectionId}")]
    public async Task<IActionResult> UpdateAccessBySectionAndRole(string roleId, long sectionId,
        [FromBody] AccessUpdateDto accessUpdateDto)
    {
        // Check if the accessDto has matching RoleId and SectionId
        if (roleId != accessUpdateDto.RoleId || sectionId != accessUpdateDto.SectionId) return BadRequest();

        var access = _mapper.Map<Access>(accessUpdateDto);
        await _repository.UpdateAccessBySectionAndRole(roleId, sectionId, access);
        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAccess(long id)
    {
        var access = await _repository.GetAccessById(id);
        if (access == null) return NotFound();
        await _repository.DeleteAccess(id);
        return NoContent();
    }

    [HttpGet("{roleId}/accesses")]
    public async Task<ActionResult<Dictionary<string, object>>> GetRoleAccesses(string roleId)
    {
        var accesses = await _repository.GetRoleAccesses(roleId);

        if (accesses == null) return NotFound();

        return Ok(accesses);
    }
}