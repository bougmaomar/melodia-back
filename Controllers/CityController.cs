using AutoMapper;
using melodia_api.Models;
using melodia_api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace melodia_api.Controllers;

[Route("api/cities")]
[ApiController]
public class CityController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ICityRepository _repository;
    
    public CityController(ICityRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    
    [HttpGet("country/{countryId:int}")]
    public async Task<ActionResult<IEnumerable<CityViewDto>>> GetCitiesByCountry(long countryId)
    {
        var cities = await _repository.FindCitiesByCountry(countryId);
        if (cities.Count == 0) return new NotFoundResult();
        var citiesView = _mapper.Map<List<CityViewDto>>(cities);
        return Ok(citiesView);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<IEnumerable<CityViewDto>>> GetCityById(long id)
    {
        var city = await _repository.FindCityById(id);
        var cityView = _mapper.Map<CityViewDto>(city);
        return Ok(cityView);
    }
}