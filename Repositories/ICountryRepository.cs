using melodia.Entities;

namespace melodia_api.Repositories;

public interface ICountryRepository
{
    public Task<List<Country>> GetAllCountries();
}