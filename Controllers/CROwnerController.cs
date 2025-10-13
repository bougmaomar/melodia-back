using AutoMapper;
using melodia.Entities;
using melodia_api.Models.SongCROwner;
using melodia_api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace melodia_api.Controllers
{
    [Route("api/[controller]s")]
    [ApiController]
    public class CROwnerController : ControllerBase
    {
        public readonly IMapper _mapper;
        public readonly ICROwnerRepository _repository;

        public CROwnerController(IMapper mapper, ICROwnerRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CROwnerViewDto>>> GetAllCROwners()
        {
            var owners = _mapper.Map<List<CROwnerViewDto>>(await _repository.GetAllCROwners());
            return Ok(owners);
        }

        [HttpGet("{ownerId:long}")]
        public async Task<ActionResult<CROwnerViewDto>> FindCROwnerById(long id)
        {
            var owner = _mapper.Map<CROwnerViewDto>(await _repository.FindCROwnerById(id));
            return Ok(owner);
        }

        [HttpPost]
        public async Task<ActionResult<CROwnerViewDto>> CreateCROwner([FromBody] CROwnerCreateDto ownerCreate)
        {
            if (!ModelState.IsValid) return BadRequest();
            var owner = _mapper.Map<CROwner>(ownerCreate);
            var createdOwner = await _repository.CreateCROwner(ownerCreate);
            var createdOwnerView = _mapper.Map<CROwnerViewDto>(createdOwner);
            return new CreatedAtActionResult(nameof(FindCROwnerById), "CROwner", new { ownerId = createdOwnerView.Id }, createdOwnerView);
        }

        [HttpPut]
        public async Task<ActionResult<CROwnerViewDto>> UpdateSongCROwner([FromBody] CROwnerUpdateDto ownerUpdate)
        {
            if (!ModelState.IsValid) return BadRequest();
            var owner = _mapper.Map<CROwner>(ownerUpdate);
            var updatedOwner = await _repository.UpdateCROwner(owner);
            var updatedOwnerView = _mapper.Map<CROwnerViewDto>(updatedOwner);
            return new CreatedAtActionResult(nameof(FindCROwnerById), "CROwner", new { ownerId = updatedOwnerView.Id }, updatedOwnerView);
        }
        
        [HttpPut("desactivate")]
        public async Task<ActionResult> DesactivateCROwnerById(long id)
        {
            await _repository.DesactivateCROwnerById(id);
            return NoContent();
        }
        
        [HttpPut("activate")]
        public async Task<ActionResult> ActivateCROwnerById(long id)
        {
            await _repository.ActivateCROwnerById(id);
            return NoContent();
        }

    }
}
