using AutoMapper;
using melodia_api.Models.SongCROwner;
using melodia_api.Repositories;
using melodia.Entities;
using Microsoft.AspNetCore.Mvc;

namespace melodia_api.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class SongCROwnerController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ISongCROwnerRepository _repository;
    
    public SongCROwnerController(ISongCROwnerRepository repository, IMapper mapper) { _repository = repository; _mapper = mapper; }
    
    [HttpGet("{songCROwnerId:long}")]
    public async Task<ActionResult<SongCROwnerViewDto>> GetSongCROwnerById(long songCROwnerId)
    {
        var songCROwner = _mapper.Map<SongCROwnerViewDto>(await _repository.FindSongCROwnerById(songCROwnerId));
        return Ok(songCROwner);
    } 
    
    [HttpPost]
    public async Task<ActionResult<SongCROwnerViewDto>> CreateSongCROwner([FromBody] SongCROwnerCreateDto songCrOwnerCreate)
    {
        if (!ModelState.IsValid) return BadRequest();
        //var songCROwner = _mapper.Map<SongCROwner>(songCrOwnerCreate);
        var createdSongCROwner = await _repository.CreateSongCROwner(songCrOwnerCreate);
        var songCROwnerView = _mapper.Map<SongCROwnerViewDto>(createdSongCROwner);
        return new CreatedAtActionResult(nameof(GetSongCROwnerById), "SongCROwner", new { SongCROwnerId = songCROwnerView.Id }, songCROwnerView);
    }
    
    [HttpPut]
    public async Task<ActionResult<SongCROwnerViewDto>> UpdateSongCROwner([FromBody] SongCROwnerUpdateDto songCROwnerUpdateDto)
    {
        if (!ModelState.IsValid) return BadRequest();
        //var songCROwner = _mapper.Map<SongCROwner>(SongCROwnerUpdateDto);
        var updatedSongCROwnerView = _mapper.Map<SongCROwnerViewDto>(await _repository.UpdateSongCROwner(songCROwnerUpdateDto));
        return new CreatedAtActionResult(nameof(GetSongCROwnerById), "SongCROwner", new { songCROwnerId = updatedSongCROwnerView.Id }, updatedSongCROwnerView);
    }
    
    [HttpDelete("{songCROwnerId:long}")]
    public async Task<ActionResult> DeactivateSongCROwnerById(long songCROwnerId)
    {
        await _repository.DeactivateSongCROwnerById(songCROwnerId);
        return NoContent();
    }
    
}