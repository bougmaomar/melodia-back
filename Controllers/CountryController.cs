using AutoMapper;
using melodia_api.Models;
using melodia_api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace melodia_api.Controllers;

[Route("api/countries")]
[ApiController]
public class CountryController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ICountryRepository _repository;
    
    public CountryController(ICountryRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CountryViewDto>>> GetAllCountries()
    {
        var countries = await _repository.GetAllCountries();
        if (countries.Count == 0) return new NotFoundResult();
        return Ok(_mapper.Map<List<CountryViewDto>>(countries));
    }
    
}