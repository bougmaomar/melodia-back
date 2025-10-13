using AutoMapper;
using melodia_api.Entities;
using melodia_api.Models.Section;
using melodia_api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace melodia_api.Controllers;

[Route("api/sections")]
[ApiController]
public class SectionController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ISectionRepository _repository;

    public SectionController(ISectionRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    
    private async Task defaultSections()
    {
        var defaultSections = new List<string>
        {
            "Dashboard",
            "Songs",
            "Albums",
            "Sections",
            "Roles",
            "Accounts",
            "Manage Access",
        };

        foreach (var label in defaultSections)
        {
            if (!_repository.ExistsByLabel(label))
            {
                var section = new Section { Label = label };
                _repository.Add(section);
            }
        }

        await _repository.SaveChangesAsync();
    }
    
      [HttpGet("role")]
        public async Task<ActionResult<IEnumerable<SectionViewDto>>> GetAllSectionsByEnterpriseIdAndRole(string roleId)
        {
            var sections = await _repository.GetAllSectionsByRole(roleId);
            return Ok(_mapper.Map<IEnumerable<SectionViewDto>>(sections));
        }
        
        [HttpPost("default-sections")]
        public async Task<IActionResult> CreateDefaultSections()
        {
            try
            {
                await defaultSections();
                return Ok("Sections par défaut créées avec succès.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Une erreur est survenue lors de la création des sections par défaut.");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SectionViewDto>>> GetAllSections()
        {
            var sections = await _repository.GetAllSections();
            return Ok(_mapper.Map<IEnumerable<SectionViewDto>>(sections));
        }

        [HttpGet("ParentSections")]
        public async Task<ActionResult<IEnumerable<SectionViewDto>>> GetParentSections()
        {
            var parentSections = await _repository.GetParentSections();
            var parentSectionDtos = _mapper.Map<IEnumerable<SectionViewDto>>(parentSections);
            return Ok(parentSectionDtos);
        }

        [HttpGet("SubSections/{parentId}")]
        public async Task<ActionResult<IEnumerable<SectionViewDto>>> GetSubSectionsByParentId(long parentId)
        {
            var subSections = await _repository.GetSubSectionsByParentId(parentId);
            var subSectionDtos = _mapper.Map<IEnumerable<SectionViewDto>>(subSections);
            return Ok(subSectionDtos);
        }

        [HttpGet("SectionsByRole/{roleId}")]
        public async Task<ActionResult<(List<SectionViewDto> ParentSections, List<SectionViewDto> SubSections)>> GetSectionsByRole(string roleId)
        {
            var (parentSections, subSections) = await _repository.GetSectionsByRole(roleId);
            if (parentSections == null || subSections == null)
            {
                return NotFound();
            }

            var parentSectionDtos = _mapper.Map<List<SectionViewDto>>(parentSections);
            var subSectionDtos = _mapper.Map<List<SectionViewDto>>(subSections);

            return Ok(new
            {
                ParentSections = parentSectionDtos,
                SubSections = subSectionDtos
            });
        }

        [HttpGet("parent/{parentId}/subsections")]
        public async Task<ActionResult<IEnumerable<SectionViewDto>>> GetSubSections(long parentId)
        {
            var sections = await _repository.GetSubSections(parentId);
            var sectionViewDtos = _mapper.Map<IEnumerable<SectionViewDto>>(sections);

            return Ok(sectionViewDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SectionViewDto>> GetSectionById(long id)
        {
            var section = await _repository.GetSectionById(id);
            if (section == null) return NotFound();
            return Ok(_mapper.Map<SectionViewDto>(section));
        }

        [HttpPost]
        public async Task<ActionResult<SectionViewDto>> CreateSection(SectionCreateDto sectionCreateDto, long? parentId)
        {
            Section parentSection = null;
            if (sectionCreateDto.ParentSectionId.HasValue)
            {
                parentSection = await _repository.GetSectionById(sectionCreateDto.ParentSectionId.Value);
                if (parentSection == null)
                {
                    return NotFound("Parent section does not exist.");
                }
            }

            var section = _mapper.Map<Section>(sectionCreateDto);
            section.ParentSection = parentSection;
            await _repository.AddSection(section);

            var sectionViewDto = _mapper.Map<SectionViewDto>(section);
            return CreatedAtAction(nameof(GetSectionById), new { id = sectionViewDto.Id }, sectionViewDto);
        }

        [HttpPost("subsections")]
        public async Task<ActionResult<SectionViewDto>> AddSubSection(SectionCreateDto subSectionCreateDto)
        {
            if (subSectionCreateDto == null)
            {
                return BadRequest("The subsection data is missing or invalid.");
            }

            var parentSection = await _repository.GetSectionById(subSectionCreateDto.ParentSectionId);
            if (parentSection == null)
            {
                return NotFound("Parent section does not exist.");
            }

            if (parentSection.ParentSectionId != null)
            {
                return BadRequest("Subsections can only be added to sections with parentId equal to null.");
            }

            if (parentSection.SubSections != null && parentSection.SubSections.Any(s => s.Label == subSectionCreateDto.Label))
            {
                return BadRequest("A subsection with the same label already exists in the parent section.");
            }

            var subSection = _mapper.Map<Section>(subSectionCreateDto);
            parentSection.SubSections ??= new List<Section>();
            parentSection.SubSections.Add(subSection);
            await _repository.SaveChangesAsync();

            var subSectionViewDto = _mapper.Map<SectionViewDto>(subSection);
            return CreatedAtAction(nameof(GetSectionById), new { id = subSectionViewDto.Id }, subSectionViewDto);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSection(long id, SectionUpdateDto sectionUpdateDto)
        {
            var section = await _repository.GetSectionById(id);
            if (section == null) return NotFound();
            _mapper.Map(sectionUpdateDto, section);
            await _repository.UpdateSection(section);
            return NoContent();
        }

        [HttpPut("{parentId}/subsections/{subSectionId}")]
        public async Task<IActionResult> UpdateSubSectionLabel(long parentId, long subSectionId, SectionUpdateDto subSectionUpdateDto)
        {
            var parentSection = await _repository.GetSectionById(parentId);
            if (parentSection == null)
            {
                return NotFound("Parent section does not exist.");
            }

            var subSection = parentSection.SubSections?.FirstOrDefault(s => s.Id == subSectionId);
            if (subSection == null)
            {
                return NotFound("Subsection does not exist.");
            }

            subSection.Label = subSectionUpdateDto.Label;
            await _repository.UpdateSection(subSection);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSection(long id)
        {
            var section = await _repository.GetSectionById(id);
            if (section == null) return NotFound();
            await _repository.DeleteSection(id);
            return NoContent();
        }


}