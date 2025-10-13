using AutoMapper;
using melodia_api.Models.StationType;
using melodia_api.Repositories;
using melodia.Entities;
using Microsoft.AspNetCore.Mvc;

namespace melodia_api.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class StationTypeController : ControllerBase
{
	private readonly IMapper _mapper;
	private readonly IStationTypeRepository _repository;

	public StationTypeController(IStationTypeRepository repository, IMapper mapper)
	{
		_repository = repository;
		_mapper = mapper;
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<StationTypeViewDto>>> GetAllStationTypes()
	{
		var stationTypes = _mapper.Map<List<StationTypeViewDto>>(await _repository.GetAllStationTypes());
		return Ok(stationTypes);
	}

	[HttpPost]
	public async Task<ActionResult<StationTypeViewDto>> CreateStationType(
		[FromBody] StationTypeCreateDto stationTypeCreate)
	{
		if (!ModelState.IsValid) return BadRequest();
		var stationType = _mapper.Map<StationType>(stationTypeCreate);
		var createdStationType = await _repository.CreateStationType(stationType);
		var stationTypeView = _mapper.Map<StationTypeViewDto>(createdStationType);
		return new CreatedAtActionResult(nameof(GetStationTypeById), "StationType",
			new { stationTypeId = stationTypeView.Id }, stationTypeView);
	}

	[HttpPut]
	public async Task<ActionResult<StationTypeViewDto>> UpdateStationType(
		[FromBody] StationTypeUpdateDto stationTypeUpdate)
	{
		if (!ModelState.IsValid) return BadRequest();
		var stationType = _mapper.Map<StationType>(stationTypeUpdate);
		var updatedStationTypeView =
			_mapper.Map<StationTypeViewDto>(await _repository.UpdateStationType(stationType));
		return new CreatedAtActionResult(nameof(GetStationTypeById), "StationType",
			new { stationTypeId = updatedStationTypeView.Id }, updatedStationTypeView);
	}

	[HttpGet("{stationTypeId:long}")]
	public async Task<ActionResult<StationTypeViewDto>> GetStationTypeById(long stationTypeId)
	{
		var stationType = _mapper.Map<StationTypeViewDto>(await _repository.FindStationTypeById(stationTypeId));
		return Ok(stationType);
	}
	
	[HttpPut("deactivate/{id}")]
	public async Task<IActionResult> DesactivateStationType(long id)
	{
		var result = await _repository.DesactivateStationType(id);
		if (!result) return NotFound();

		return NoContent(); 
	}

	[HttpPut("activate/{id}")]
	public async Task<IActionResult> ActivateStationType(long id)
	{
		var result = await _repository.ActivateStationType(id);
		if (!result) return NotFound();

		return NoContent(); 
	}
    
	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteStationType(long id)
	{
		var result = await _repository.DeleteStationType(id);
		if (!result) return NotFound();

		return NoContent(); 
	}
}