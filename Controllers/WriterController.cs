using AutoMapper;
using melodia.Entities;
using melodia_api.Models.SongWriter;
using melodia_api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace melodia_api.Controllers

{
    [Route("api/[controller]s")]
    [ApiController]
    public class WriterController : ControllerBase
    {
        public readonly IMapper _mapper;
        public readonly IWriterRepository _repository;

        public WriterController(IMapper mapper, IWriterRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WriterViewDto>>> GetAllWriters()
        {
            var writers = _mapper.Map<List<WriterViewDto>>(await _repository.GetAllWriters());
            return Ok(writers);
        }

        [HttpGet("{writerId:long}")]
        public async Task<ActionResult<WriterViewDto>> FindWriterById(long id)
        {
            var writer = _mapper.Map<WriterViewDto>(await _repository.FindWriterById(id));
            return Ok(writer);
        }

        [HttpPost]
        public async Task<ActionResult<WriterViewDto>> CreateWriter([FromBody] WriterCreateDto writerCreate)
        {
            if (!ModelState.IsValid) return BadRequest();
            var writer = _mapper.Map<Writer>(writerCreate);
            var createdWriter = await _repository.CreateWriter(writerCreate);
            var createdWriterView = _mapper.Map<WriterViewDto>(createdWriter);
            return new CreatedAtActionResult(nameof(FindWriterById), "Writer", new { writerId = createdWriterView.Id }, createdWriterView);
        }

        [HttpPut]
        public async Task<ActionResult<WriterViewDto>> UpdateWriter([FromBody] WriterUpdateDto writerUpdate)
        {
            if (!ModelState.IsValid) return BadRequest();
            var writer = _mapper.Map<Writer>(writerUpdate);
            var updatedWriter = await _repository.UpdateWriter(writer);
            var updatedWriterView = _mapper.Map<WriterViewDto>(updatedWriter);
            return new CreatedAtActionResult(nameof(FindWriterById), "Writer", new { writerId = updatedWriterView.Id }, updatedWriterView);
        }
        
        [HttpPut("desactivate")]
        public async Task<ActionResult> DesactivateWriterById(long id)
        {
            await _repository.DesactivateWriterById(id);
            return NoContent();
        }
        
        [HttpPut("activate")]
        public async Task<ActionResult> ActivateWriterById(long id)
        {
            await _repository.ActivateWriterById(id);
            return NoContent();
        }

    }
}
