using melodia.Configurations;
using melodia.Entities;
using Microsoft.EntityFrameworkCore;

namespace melodia_api.Repositories.Implementations;

public class CountryRepository : ICountryRepository
{
    private readonly MelodiaDbContext _db;

    public CountryRepository(MelodiaDbContext db)
    {
        _db = db;
    }

    public Task<List<Country>> GetAllCountries()
    {
        return _db.Countries.ToListAsync();
    }
}