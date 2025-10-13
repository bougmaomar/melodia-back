using melodia.Configurations;
using melodia.Entities;
using Microsoft.EntityFrameworkCore;

namespace melodia_api.Repositories.Implementations;

public class CityRepository : ICityRepository
{
    private readonly MelodiaDbContext _db;

    public CityRepository(MelodiaDbContext db) { _db = db; }

    public Task<List<City>> FindCitiesByCountry(long countryId)
    {
        return _db.Cities.Include(c => c.Country).Where(city => city.CountryId == countryId).ToListAsync();
    }
    public Task<City> FindCityById(long id)
    {
        return _db.Cities.Include(c => c.Country)
                        .FirstOrDefaultAsync(city => city.Id == id);

    }
}