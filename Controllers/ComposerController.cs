using AutoMapper;
using melodia.Entities;
using melodia_api.Models.SongComposer;
using melodia_api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace melodia_api.Controllers
{
    [Route("api/[controller]s")]
    [ApiController]
    public class ComposerController : ControllerBase
    {
        public readonly IMapper _mapper;
        public readonly IComposerRepository _repository;

        public ComposerController(IMapper mapper, IComposerRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ComposerViewDto>>> GetAllComposers()
        {
            var songComposers = _mapper.Map<List<ComposerViewDto>>(await _repository.GetAllComposers());
            return Ok(songComposers);
        }

        [HttpGet("{composerId:long}")]
        public async Task<ActionResult<ComposerViewDto>> FindComposerById(long id)
        {
            var composer = _mapper.Map<ComposerViewDto>(await _repository.FindComposerById(id));
            return Ok(composer);
        }

        [HttpPost]
        public async Task<ActionResult<ComposerViewDto>> CreateComposer([FromBody] ComposerCreateDto composerCreate)
        {
            if (!ModelState.IsValid) return BadRequest();
            var composer = _mapper.Map<Composer>(composerCreate);
            var createdComposer = await _repository.CreateComposer(composerCreate);
            var createdComposerView = _mapper.Map<ComposerViewDto>(createdComposer);
            return new CreatedAtActionResult(nameof(FindComposerById), "Composer", new { composerId = createdComposerView.Id }, createdComposerView);
        }

        [HttpPut]
        public async Task<ActionResult<ComposerViewDto>> UpdateComposer([FromBody] ComposerUpdateDto composerUpdate)
        {
            if (!ModelState.IsValid) return BadRequest();
            var composer = _mapper.Map<Composer>(composerUpdate);
            var updatedComposer = await _repository.UpdateComposer(composer);
            var updatedComposerView = _mapper.Map<ComposerViewDto>(updatedComposer);
            return new CreatedAtActionResult(nameof(FindComposerById), "Composer", new { composerId = updatedComposerView.Id }, updatedComposerView);
        }
        
        [HttpPut("desactivate")]
        public async Task<ActionResult> DesactivateComposerById(long id)
        {
            await _repository.DesactivateComposerById(id);
            return NoContent();
        }
        
        [HttpPut("activate")]
        public async Task<ActionResult> ActivateComposerById(long id)
        {
            await _repository.ActivateComposerById(id);
            return NoContent();
        }

    }
}
