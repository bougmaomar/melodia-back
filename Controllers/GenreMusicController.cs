using AutoMapper;
using melodia_api.Models.GenreMusic;
using melodia_api.Repositories;
using melodia.Entities;
using Microsoft.AspNetCore.Mvc;

namespace melodia_api.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class GenreMusicController : ControllerBase
{
    public readonly IMapper _mapper;
    public readonly IGenreMusicRepository _repository;

    public GenreMusicController(IMapper mapper, IGenreMusicRepository repository)
    {
        _mapper = mapper;
        _repository = repository;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GenreMusicViewDto>>> GetAllGenreMusics()
    {
        var genreMusics = _mapper.Map<List<GenreMusicViewDto>>(await _repository.GetAllGenreMusics());
        return Ok(genreMusics);
    }
		
    [HttpGet("{genreMusicId:long}")]
    public async Task<ActionResult<GenreMusicViewDto>> GetGenreMusicById(long id)
    {
        var genreMusic = _mapper.Map<GenreMusicViewDto>(await _repository.FindGenreMusicById(id));
        return Ok(genreMusic);
    }
    
    [HttpPost]
    public async Task<ActionResult<GenreMusicViewDto>> CreateGenreMusic([FromBody] GenreMusicCreateDto genreMusicCreate)
    {
        if (!ModelState.IsValid) return BadRequest();
        var genreMusic = _mapper.Map<GenreMusic>(genreMusicCreate);
        var createdGenreMusic = await _repository.CreateGenreMusic(genreMusic);
        var createdGenreMusicView = _mapper.Map<GenreMusicViewDto>(createdGenreMusic);
        return new CreatedAtActionResult(nameof(GetGenreMusicById), "GenreMusic", new { genreMusicId = genreMusic.Id }, createdGenreMusicView);
    }
    
    [HttpPut]
    public async Task<ActionResult<GenreMusicViewDto>> UpdateGenreMusic([FromBody] GenreMusicUpdateDto genreMusicUpdate)
    {
        if (!ModelState.IsValid) return BadRequest();
        var genreMusic = _mapper.Map<GenreMusic>(genreMusicUpdate);
        var updatedGenreMusic = await _repository.UpdateGenreMusic(genreMusic);
        var updatedGenreMusicView = _mapper.Map<GenreMusicViewDto>(updatedGenreMusic);
        return new CreatedAtActionResult(nameof(GetGenreMusicById), "GenreMusic", new { genreMusicId = updatedGenreMusicView.Id }, updatedGenreMusicView);
    }
    
    [HttpPut("deactivate/{id}")]
    public async Task<IActionResult> DesactivateGenreMusic(long id)
    {
        var result = await _repository.DesactivateGenreMusic(id);
        if (!result) return NotFound();

        return NoContent(); 
    }

    [HttpPut("activate/{id}")]
    public async Task<IActionResult> ActivateGenreMusic(long id)
    {
        var result = await _repository.ActivateGenreMusic(id);
        if (!result) return NotFound();

        return NoContent(); 
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGenre(long id)
    {
        var result = await _repository.DeleteGenreMusic(id);
        if (!result) return NotFound();

        return NoContent(); 
    }
}