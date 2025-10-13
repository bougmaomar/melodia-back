using AutoMapper;
using melodia_api.Models.SongPOwner;
using melodia_api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace melodia_api.Controllers
{
    [Route("api/[controller]s")]
    [ApiController]
    public class SongPOwnerController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISongPOwnerRepository _repository;

        public SongPOwnerController(ISongPOwnerRepository repository, IMapper mapper) { _repository = repository; _mapper = mapper; }

        [HttpGet("{songPOwnerId:long}")]
        public async Task<ActionResult<SongPOwnerViewDto>> GetSongPOwnerById(long songPOwnerId)
        {
            var songPOwner = _mapper.Map<SongPOwnerViewDto>(await _repository.FindSongPOwnerById(songPOwnerId));
            return Ok(songPOwner);
        }

        [HttpPost]
        public async Task<ActionResult<SongPOwnerViewDto>> CreateSongPOwner([FromBody] SongPOwnerCreateDto songPOwnerCreate)
        {
            if (!ModelState.IsValid) return BadRequest();
            var createdSongPOwner = await _repository.CreateSongPOwner(songPOwnerCreate);
            var songPOwnerView = _mapper.Map<SongPOwnerViewDto>(createdSongPOwner);
            return new CreatedAtActionResult(nameof(GetSongPOwnerById), "SongPOwner", new { SongPOwnerId = songPOwnerView.Id }, songPOwnerView);
        }

        [HttpPut]
        public async Task<ActionResult<SongPOwnerViewDto>> UpdateSongPOwner([FromBody] SongPOwnerUpdateDto songPOwnerUpdateDto)
        {
            if (!ModelState.IsValid) return BadRequest();
            var updatedSongPOwnerView = _mapper.Map<SongPOwnerViewDto>(await _repository.UpdateSongPOwner(songPOwnerUpdateDto));
            return new CreatedAtActionResult(nameof(GetSongPOwnerById), "SongPOwner", new { songPOwnerId = updatedSongPOwnerView.Id }, updatedSongPOwnerView);
        }

        [HttpDelete("{songPOwnerId:long}")]
        public async Task<ActionResult> DeactivateSongPOwnerById(long songPOwnerId)
        {
            await _repository.DeactivateSongPOwnerById(songPOwnerId);
            return NoContent();
        }
    }
}
