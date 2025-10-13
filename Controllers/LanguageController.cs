using AutoMapper;
using DocumentFormat.OpenXml.Wordprocessing;
using melodia.Entities;
using melodia_api.Models.Language;
using melodia_api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace melodia_api.Controllers
{
	[Route("api/[controller]s")]
	[ApiController]
	public class LanguageController : ControllerBase
	{
		public readonly IMapper _mapper;
		public readonly ILanguageRepository _repository;

		public LanguageController(IMapper mapper, ILanguageRepository repository)
		{
			_mapper = mapper;
			_repository = repository;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<LanguageViewDto>>> GetAllLanguages()
		{
			var languages = _mapper.Map<List<LanguageViewDto>>(await _repository.GetAllLanguages());
			return Ok(languages);
		}
		
		[HttpGet("{languageId:long}")]
		public async Task<ActionResult<LanguageViewDto>> GetLanguageById(long id)
		{
			var language = _mapper.Map<LanguageViewDto>(await _repository.FindLanguageById(id));
			return Ok(language);
		}
		[HttpPost]
		public async Task<ActionResult<LanguageViewDto>> CreateLanguage([FromBody] LanguageCreateDto languageCreate)
		{
			if (!ModelState.IsValid) return BadRequest();
			var language = _mapper.Map<Language>(languageCreate);
			var createdLanguage = await _repository.CreateLanguage(language);
			var createdLanguageView = _mapper.Map<LanguageViewDto>(createdLanguage);
			return new CreatedAtActionResult(nameof(GetLanguageById), "Language", new { languageId = language.Id }, createdLanguageView);
		}
		[HttpPut]
		public async Task<ActionResult<LanguageViewDto>> UpdateLanguage([FromBody] LanguageUpdateDto languageUpdate)
		{
			if (!ModelState.IsValid) return BadRequest();
			var language = _mapper.Map<Language>(languageUpdate);
			var updatedLanguage = await _repository.UpdateLanguage(language);
			var updatedLanguageView = _mapper.Map<LanguageViewDto>(updatedLanguage);
			return new CreatedAtActionResult(nameof(GetLanguageById), "Language", new { languageId = updatedLanguageView.Id }, updatedLanguageView);
		}

		[HttpPut("deactivate/{id}")]
		public async Task<IActionResult> DesactivateLanguage(long id)
		{
			var result = await _repository.DesactivateLanguage(id);
			if (!result) return NotFound();

			return NoContent(); 
		}

		[HttpPut("activate/{id}")]
		public async Task<IActionResult> ActivateLanguage(long id)
		{
			var result = await _repository.ActivateLanguage(id);
			if (!result) return NotFound();

			return NoContent(); 
		}
    
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteLanguage(long id)
		{
			var result = await _repository.DeleteLanguage(id);
			if (!result) return NotFound();

			return NoContent(); 
		}
	}
}
