using melodia.Entities;
namespace melodia_api.Repositories;

public interface ICityRepository
{ 
    public Task<List<City>> FindCitiesByCountry(long countryId);
    public Task<City> FindCityById(long id);

}