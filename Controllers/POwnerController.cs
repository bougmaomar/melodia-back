using AutoMapper;
using melodia.Entities;
using melodia_api.Models.POwner;
using melodia_api.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace melodia_api.Controllers
{
    [Route("api/[controller]s")]
    [ApiController]
    public class POwnerController : ControllerBase
    {
        public readonly IMapper _mapper;
        public readonly IPOwnerRepository _repository;

        public POwnerController(IMapper mapper, IPOwnerRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<POwnerViewDto>>> GetAllPOwners()
        {
            var owners = _mapper.Map<List<POwnerViewDto>>(await _repository.GetAllPOwners());
            return Ok(owners);
        }

        [HttpGet("{pownerId:long}")]
        public async Task<ActionResult<POwnerViewDto>> FindPOwnerById(long id)
        {
            var owner = _mapper.Map<POwnerViewDto>(await _repository.FindPOwnerById(id));
            return Ok(owner);
        }

        [HttpPost]
        public async Task<ActionResult<POwnerViewDto>> CreatePOwner([FromBody] POwnerCreateDto ownerCreate)
        {
            if (!ModelState.IsValid) return BadRequest();
            var owner = _mapper.Map<POwner>(ownerCreate);
            var createdOwner = await _repository.CreatePOwner(ownerCreate);
            var createdOwnerView = _mapper.Map<POwnerViewDto>(createdOwner);
            return new CreatedAtActionResult(nameof(FindPOwnerById), "POwner", new { pOwnerId = createdOwnerView.Id }, createdOwnerView);
        }

        [HttpPut]
        public async Task<ActionResult<POwnerViewDto>> UpdateSongPOwner([FromBody] POwnerUpdateDto ownerUpdate)
        {
            if (!ModelState.IsValid) return BadRequest();
            var owner = _mapper.Map<POwner>(ownerUpdate);
            var updatedOwner = await _repository.UpdatePOwner(owner);
            var updatedOwnerView = _mapper.Map<POwnerViewDto>(updatedOwner);
            return new CreatedAtActionResult(nameof(FindPOwnerById), "POwner", new { pOwnerId = updatedOwnerView.Id }, updatedOwnerView);
        }

        [HttpPut("desactivate")]
        public async Task<ActionResult> DesactivatePOwnerById(long id)
        {
            await _repository.DesactivatePOwnerById(id);
            return NoContent();
        }

        [HttpPut("activate")]
        public async Task<ActionResult> ActivatePOwnerById(long id)
        {
            await _repository.ActivatePOwnerById(id);
            return NoContent();
        }

    }
}
