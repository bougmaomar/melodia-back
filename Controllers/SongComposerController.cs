using AutoMapper;
using melodia_api.Models.SongComposer;
using melodia_api.Repositories;
using melodia.Entities;
using Microsoft.AspNetCore.Mvc;

namespace melodia_api.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class SongComposerController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ISongComposerRepository _repository;
    
    public SongComposerController(ISongComposerRepository repository, IMapper mapper) { _repository = repository; _mapper = mapper; }
    
    [HttpGet("{songComposerId:long}")]
    public async Task<ActionResult<SongComposerViewDto>> GetSongComposerById(long songComposerId)
    {
        var songComposer = _mapper.Map<SongComposerViewDto>(await _repository.FindSongComposerById(songComposerId));
        return Ok(songComposer);
    } 
    
    [HttpPost]
    public async Task<ActionResult<SongComposerViewDto>> CreateSongComposer([FromBody] SongComposerCreateDto songComposerCreateDto)
    {
        if (!ModelState.IsValid) return BadRequest();
        //var songComposer = _mapper.Map<SongComposer>(songComposerCreateDto);
        var createdSongComposer = await _repository.CreateSongComposer(songComposerCreateDto);
        var songComposerView = _mapper.Map<SongComposerViewDto>(createdSongComposer);
        return new CreatedAtActionResult(nameof(GetSongComposerById), "SongComposer", new { songComposerId = songComposerView.Id }, songComposerView);
    }
    
    [HttpPut]
    public async Task<ActionResult<SongComposerViewDto>> UpdateSongComposer([FromBody] SongComposerUpdateDto songComposerUpdateDto)
    {
        if (!ModelState.IsValid) return BadRequest();
        //var songComposer = _mapper.Map<SongComposer>(songComposerUpdateDto);
        var updatedSongComposerView = _mapper.Map<SongComposerViewDto>(await _repository.UpdateSongComposer(songComposerUpdateDto));
        return new CreatedAtActionResult(nameof(GetSongComposerById), "SongComposer", new { songComposerId = updatedSongComposerView.Id }, updatedSongComposerView);
    }
    
    [HttpDelete("{songComposerId:long}")]
    public async Task<ActionResult> DeactivateSongComposerById(long songComposerId)
    {
        await _repository.DeactivateSongComposerById(songComposerId);
        return NoContent();
    }
    
}