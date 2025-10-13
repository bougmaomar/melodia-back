using AutoMapper;
using melodia.Entities;
using melodia_api.Models.Position;
using melodia_api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace melodia_api.Controllers
{
	[Route("api/[controller]s")]
	[ApiController]
	public class PositionController : ControllerBase
	{
		public readonly IMapper _mapper;
		public readonly IPositionRepository _repository;

		public PositionController(IMapper mapper, IPositionRepository repository)
		{
			_mapper = mapper;
			_repository = repository;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<PositionViewDto>>> GetAllPositions()
		{
			var positions = _mapper.Map<List<PositionViewDto>>(await _repository.GetAllPositions());
			return Ok(positions);
		}
		[HttpGet("{positionId:long}")]
		public async Task<ActionResult<PositionViewDto>> GetPositionById(long positionId)
		{
			var position = _mapper.Map<PositionViewDto>(await _repository.FindPositionById(positionId));
			return Ok(position);
		}
		[HttpPost]
		public async Task<ActionResult<PositionViewDto>> CreatePosition([FromBody] PositionCreateDto positionCreate)
		{
			if (!ModelState.IsValid) return BadRequest();
			var position = _mapper.Map<Position>(positionCreate);
			var createdPosition = await _repository.CreatePosition(position);
			var positionView = _mapper.Map<PositionViewDto>(createdPosition);
			return new CreatedAtActionResult(nameof(GetPositionById), "Position",
		   new { positionId = position.Id }, positionView);
		}
		[HttpPut]
		public async Task<ActionResult<PositionViewDto>> UpdatePosition([FromBody] PositionUpdateDto positionUpdate)
		{
			if (!ModelState.IsValid) return BadRequest();
			var position = _mapper.Map<Position>(positionUpdate);
			var updatedPositionView = _mapper.Map<PositionViewDto>(await _repository.UpdatePosition(position));
			return new CreatedAtActionResult(nameof(GetPositionById), "Position", new { positionId = updatedPositionView.Id }, updatedPositionView);
		}
		
		[HttpPut("deactivate/{id}")]
		public async Task<IActionResult> DesactivatePosition(long id)
		{
			var result = await _repository.DesactivatePosition(id);
			if (!result) return NotFound();

			return NoContent(); 
		}

		[HttpPut("activate/{id}")]
		public async Task<IActionResult> ActivatePosition(long id)
		{
			var result = await _repository.ActivatePosition(id);
			if (!result) return NotFound();

			return NoContent(); 
		}
    
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeletePosition(long id)
		{
			var result = await _repository.DeletePosition(id);
			if (!result) return NotFound();

			return NoContent(); 
		}
	}
}
