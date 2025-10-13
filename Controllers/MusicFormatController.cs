using AutoMapper;
using melodia.Entities;
using melodia_api.Models.MusicFormat;
using melodia_api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace melodia_api.Controllers
{
	[Route("api/[controller]s")]
	[ApiController]
	public class MusicFormatController : ControllerBase
	{
		public readonly IMapper _mapper;
		public readonly IMusicFormatRepository _repository;

		public MusicFormatController(IMapper mapper, IMusicFormatRepository repository)
		{
			_mapper = mapper;
			_repository = repository;
		}
		[HttpGet]
		public async Task<ActionResult<IEnumerable<MusicFormatViewDto>>> GetAllMusicFormats()
		{
			var musicFormats = _mapper.Map<List<MusicFormatViewDto>>(await _repository.GetAllMusicFormats());
			return Ok(musicFormats);
		}
		[HttpGet("{musicFormatId:long}")]
		public async Task<ActionResult<MusicFormatViewDto>> GetMusicFormatById(long musicFormatId)
		{
			var musicFormat = _mapper.Map<MusicFormatViewDto>(await _repository.FindMusicFormatById(musicFormatId));
			return Ok(musicFormat);
		}
		[HttpPost]
		public async Task<ActionResult<MusicFormatViewDto>> CreateMusicFormat([FromBody] MusicFormatCreateDto musicFormatCreate)
		{
			if (!ModelState.IsValid) return BadRequest();
			var musicFormat = _mapper.Map<MusicFormat>(musicFormatCreate);
			var createdMusicFormat = await _repository.CreateMusicFormat(musicFormat);
			var musicFormatView = _mapper.Map<MusicFormatViewDto>(createdMusicFormat);
			return new CreatedAtActionResult(nameof(GetMusicFormatById), "MusicFormat",
		   new { musicFormatId = musicFormatView.Id }, musicFormatView);
		}
		[HttpPut]
		public async Task<ActionResult<MusicFormatViewDto>> UpdateMusicFormat([FromBody] MusicFormatUpdateDto musicFormatUpdate)
		{
			if (!ModelState.IsValid) return BadRequest();
			var musicFormat = _mapper.Map<MusicFormat>(musicFormatUpdate);
			var updatedMusicFormatView = _mapper.Map<MusicFormatViewDto>(await _repository.UpdateMusicFormat(musicFormat));
			return new CreatedAtActionResult(nameof(GetMusicFormatById), "MusicFormat", new { musicFormatId = updatedMusicFormatView.Id }, updatedMusicFormatView);
		}
		[HttpDelete]
		public async Task<ActionResult> DeactivateMusicFormatById(long id)
		{
			await _repository.DesactivateMusicFormatById(id);
			return NoContent();
		}
	}
}
