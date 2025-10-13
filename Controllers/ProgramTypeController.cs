using AutoMapper;
using melodia_api.Models.ProgramType;
using melodia_api.Repositories;
using melodia.Entities;
using Microsoft.AspNetCore.Mvc;

namespace melodia_api.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class ProgramTypeController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IProgramTypeRepository _repository;

    public ProgramTypeController(IProgramTypeRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProgramTypeViewDto>>> GetAllProgramTypes()
    {
        var programTypes = _mapper.Map<List<ProgramTypeViewDto>>(await _repository.GetAllProgramTypes());
        return Ok(programTypes);
    }
    
    [HttpPost]
    	public async Task<ActionResult<ProgramTypeViewDto>> CreateProgramType(
    		[FromBody] ProgramTypeCreateDto programTypeCreateDto)
    	{
    		if (!ModelState.IsValid) return BadRequest();
    		var programType = _mapper.Map<ProgramType>(programTypeCreateDto);
    		var createdProgramType = await _repository.CreateProgramType(programType);
    		var programTypeView = _mapper.Map<ProgramTypeViewDto>(createdProgramType);
    		return new CreatedAtActionResult(nameof(GetProgramTypeById), "ProgramType",
    			new { programTypeId = programTypeView.Id }, programTypeView);
    	}
    
    	[HttpPut]
    	public async Task<ActionResult<ProgramTypeViewDto>> UpdateProgramType(
    		[FromBody] ProgramTypeUpdateDto programTypeUpdate)
    	{
    		if (!ModelState.IsValid) return BadRequest();
    		var programType = _mapper.Map<ProgramType>(programTypeUpdate);
    		var updatedProgramTypeView =
    			_mapper.Map<ProgramTypeViewDto>(await _repository.UpdateProgramType(programType));
    		return new CreatedAtActionResult(nameof(GetProgramTypeById), "ProgramType",
    			new { programTypeId = updatedProgramTypeView.Id }, updatedProgramTypeView);
    	}
    
    	[HttpGet("{programTypeId:long}")]
    	public async Task<ActionResult<ProgramTypeViewDto>> GetProgramTypeById(long programTypeId)
    	{
    		var programType = _mapper.Map<ProgramTypeViewDto>(await _repository.FindProgramTypeById(programTypeId));
    		return Ok(programType);
    	}
    	
    	[HttpPut("deactivate/{id}")]
    	public async Task<IActionResult> DesactivateProgramType(long id)
    	{
    		var result = await _repository.DesactivateProgramType(id);
    		if (!result) return NotFound();
    
    		return NoContent(); 
    	}
    
    	[HttpPut("activate/{id}")]
    	public async Task<IActionResult> ActivateProgramType(long id)
    	{
    		var result = await _repository.ActivateProgramType(id);
    		if (!result) return NotFound();
    
    		return NoContent(); 
    	}
        
    	[HttpDelete("{id}")]
    	public async Task<IActionResult> DeleteProgramType(long id)
    	{
    		var result = await _repository.DeleteProgramType(id);
    		if (!result) return NotFound();
    
    		return NoContent(); 
    	}
}