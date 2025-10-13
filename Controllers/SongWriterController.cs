using AutoMapper;
using melodia.Entities;
using melodia_api.Models.SongWriter;
using melodia_api.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace melodia_api.Controllers
{
    [Route("api/[controller]s")]
    [ApiController]
    public class SongWriterController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISongWriterRepository _repository;

        public SongWriterController(ISongWriterRepository repository, IMapper mapper) { _repository = repository; _mapper = mapper; }

        [HttpGet("{songWriterId:long}")]
        public async Task<ActionResult<SongWriterViewDto>> GetSongWriterById(long songWriterId)
        {
            var songWriter = _mapper.Map<SongWriterViewDto>(await _repository.FindSongWriterById(songWriterId));
            return Ok(songWriter);
        }

        [HttpPost]
        public async Task<ActionResult<SongWriterViewDto>> CreateSongWriter([FromBody] SongWriterCreateDto songWriterCreateDto)
        {
            if (!ModelState.IsValid) return BadRequest();
            //var songWriter = _mapper.Map<SongWriter>(songWriterCreateDto);
            var createdSongWriter = await _repository.CreateSongWriter(songWriterCreateDto);
            var songWriterView = _mapper.Map<SongWriterViewDto>(createdSongWriter);
            return new CreatedAtActionResult(nameof(GetSongWriterById), "SongWriter", new { songWriterId = songWriterView.Id }, songWriterView);
        }

        [HttpPut]
        public async Task<ActionResult<SongWriterViewDto>> UpdateSongWriter([FromBody] SongWriterUpdateDto songWriterUpdateDto)
        {
            if (!ModelState.IsValid) return BadRequest();
            //var songWriter = _mapper.Map<SongWriter>(songWriterUpdateDto);
            var updatedSongWriterView = _mapper.Map<SongWriterViewDto>(await _repository.UpdateSongWriter(songWriterUpdateDto));
            return new CreatedAtActionResult(nameof(GetSongWriterById), "SongWriter", new { songWriterId = updatedSongWriterView.Id }, updatedSongWriterView);
        }

        [HttpDelete("{songWriterId:long}")]
        public async Task<ActionResult> DeactivateSongWriterById(long songWriterId)
        {
            await _repository.DeactivateSongWriterById(songWriterId);
            return NoContent();
        }
    }
}
