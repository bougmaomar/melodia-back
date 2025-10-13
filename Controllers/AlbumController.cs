using AutoMapper;
using melodia.Entities;
using melodia_api.Models.Album;
using melodia_api.Models.Song;
using melodia_api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace melodia_api.Controllers
{
    [Route("api/[controller]s")]
    [ApiController]
    public class AlbumController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAlbumRepository _repository;

        public AlbumController(IMapper mapper, IAlbumRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        [HttpGet("filter")]
        public async Task<ActionResult<List<AlbumViewDto>>> GetFilteredAlbums(
            string title = null, 
            TimeSpan? minTotalDuration = null, 
            TimeSpan? maxTotalDuration = null, 
            long? albumTypeId = null)
        {
            try
            {
                var albums = await _repository.FilterAlbums(title, minTotalDuration, maxTotalDuration, albumTypeId);
                var albumDtos = _mapper.Map<List<AlbumViewDto>>(albums);
                return Ok(albumDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AlbumViewDto>>> GetAllAlbums()
        {
            var albums = await _repository.GetAllAlbums();
            return Ok(_mapper.Map<IEnumerable<AlbumViewDto>>(albums));
        }

        [HttpGet("artist/{artistId}")]
        public async Task<ActionResult<IEnumerable<AlbumViewDto>>> GetAlbumByArtist(long artistId)
        {
            var albums = await _repository.GetAlbumByArtist(artistId);
            return Ok(_mapper.Map<IEnumerable<AlbumViewDto>>(albums));
        }

        [HttpGet("types")]
        public async Task<ActionResult<IEnumerable<AlbumType>>> GetAlbumTypes()
        {
            var types = await _repository.GetAlbumTypes();
            return Ok(_mapper.Map<IEnumerable<AlbumType>>(types));
        }

        [HttpGet("typeId/{id}")]
        public async Task<ActionResult<IEnumerable<AlbumViewDto>>> GetAlbumsByType(long id)
        {
            var albums = await _repository.GetAlbumsByTypes(id);
            return Ok(_mapper.Map<IEnumerable<AlbumViewDto>>(albums));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AlbumViewDto>> FindAlbumById(long id)
        {
            var album = await _repository.FindAlbumById(id);

            if (album == null) return NotFound();

            return Ok(_mapper.Map<AlbumViewDto>(album));
        }

        [HttpGet("album/{id}")]
        public async Task<ActionResult<AlbumView>> GetAlbumById(long id)
        {
            var album = await _repository.GetAlbumById(id);

            if (album == null) return NotFound();

            return Ok(_mapper.Map<AlbumView>(album));
        }

        [HttpGet("relatedalbums/{id}")]
        public async Task<ActionResult<IEnumerable<AlbumViewDto>>> GetRelatedAlbums(long id)
        {
            var albums = await _repository.GetRelatedAlbums(id);
            if(albums == null) return NotFound();
            return Ok(_mapper.Map<IEnumerable<AlbumViewDto>>(albums));
        }


        [HttpPost]
        public async Task<ActionResult<AlbumViewDto>> CreateAlbum([FromForm] AlbumCreateDto albumCreateDto)
        {
            try
            {
                var album = await _repository.CreateAlbum(albumCreateDto);
                var albumViewDto = _mapper.Map<AlbumViewDto>(album);

                return CreatedAtAction(nameof(FindAlbumById), new { id = albumViewDto.Id }, albumViewDto);
            }   
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

   
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAlbum(long id, [FromForm] AlbumUpdateDto albumUpdateDto)
        {
            try
            {
                var existingAlbum = await _repository.FindAlbumById(id);
                if (existingAlbum == null) return NotFound();
                if (albumUpdateDto.CoverImage == null)
                {
                    AlbumUpdateLessDto lessDto = new AlbumUpdateLessDto();
                    var albumDto = _mapper.Map(albumUpdateDto, lessDto);
                    var updatedAlbum = _mapper.Map(albumDto, existingAlbum);

                    await _repository.UpdateAlbum(updatedAlbum, null);
                }
                else
                {
                    var updatedAlbum = _mapper.Map(albumUpdateDto, existingAlbum);

                    await _repository.UpdateAlbum(updatedAlbum, albumUpdateDto.CoverImage);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPut("{id}/deactivate")]
        public async Task<IActionResult> DeactivateAlbumById(long id)
        {
            await _repository.DeactivateAlbumById(id);
            return NoContent();
        }
        
        [HttpPut("{id}/Activate")]
        public async Task<IActionResult> ActivateAlbumById(long id)
        {
            await _repository.ActivateAlbumById(id);
            return NoContent();
        }

    }
}
