using melodia.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace melodia.Configurations;

public class GeoDataService
{
    private readonly HttpClient _httpClient;
    private readonly MelodiaDbContext _context;
    private readonly ILogger<GeoDataService> _logger;
    private readonly string _geoNamesUser = "yettafssaoui"; 
    private readonly string _apiUrl = $"http://api.geonames.org/";
    private const string MigrationId = "InitialGeographicalDataImport";

    private readonly HashSet<string> _targetCountryCodes = new HashSet<string> { "CA", "US", "ES", "IT" };

    public GeoDataService(HttpClient httpClient, MelodiaDbContext context, ILogger<GeoDataService> logger)
    {
        _httpClient = httpClient;
        _context = context;
        _logger = logger;
    }

    public async Task ImportGeographicalData()
    {
        _logger.LogInformation("Starting import of geographical data...");

        if (MigrationChecker.HasMigrationBeenApplied(_context, MigrationId))
        {
            _logger.LogInformation("Geographical data already imported.");
            return;
        }

        if (!_context.Countries.Any())
        {
            await ImportCountries();
        }
        else
        {
            var countries = await _context.Countries.ToListAsync();
            foreach (var country in countries)
            {
                if (_targetCountryCodes.Contains(country.Code))
                {
                    await ImportCities(country.Id, country.Code);
                }
            }
        }

        MigrationChecker.MarkMigrationAsApplied(_context, MigrationId);
        _logger.LogInformation("Completed import of geographical data.");
    }

    private async Task ImportCountries()
    {
        _logger.LogInformation("Importing countries...");
        var countriesResponse = await _httpClient.GetStringAsync($"{_apiUrl}countryInfoJSON?username={_geoNamesUser}");
        var countriesData = JObject.Parse(countriesResponse)["geonames"];

        if (countriesData == null)
        {
            _logger.LogError("Failed to retrieve countries data.");
            return;
        }

        foreach (var country in countriesData)
        {
            var countryName = country["countryName"]?.ToString();
            var countryCode = country["countryCode"]?.ToString();
            if (string.IsNullOrEmpty(countryName) || string.IsNullOrEmpty(countryCode))
            {
                _logger.LogWarning("Country name or country code is null or empty, skipping.");
                continue;
            }

            if (!_targetCountryCodes.Contains(countryCode))
            {
                _logger.LogInformation($"Skipping country: {countryName}");
                continue;
            }

            _logger.LogInformation($"Processing country: {countryName}");

            var countryEntity = new Country
            {
                Name = countryName,
                Code = countryCode
            };

            _context.Countries.Add(countryEntity);
            await _context.SaveChangesAsync();

            await ImportCities(countryEntity.Id, countryCode);
        }
    }

    private async Task ImportCities(long countryId, string countryCode)
    {
        if (string.IsNullOrEmpty(countryCode))
        {
            _logger.LogWarning("Country code is null or empty, skipping cities.");
            return;
        }

        _logger.LogInformation($"Importing cities for country code: {countryCode}");
        var citiesResponse = await _httpClient.GetStringAsync($"{_apiUrl}searchJSON?country={countryCode}&username={_geoNamesUser}&featureClass=P&maxRows=1000");
        
        // Log the raw response for debugging
        _logger.LogInformation($"Cities response: {citiesResponse}");

        // Check for errors in the response
        var citiesData = JObject.Parse(citiesResponse);
        if (citiesData["status"] != null)
        {
            _logger.LogError($"Error retrieving cities data: {citiesData["status"]["message"]}");
            return;
        }

        var cities = citiesData["geonames"];
        if (cities == null)
        {
            _logger.LogError("Failed to retrieve cities data.");
            return;
        }

        foreach (var city in cities)
        {
            var cityName = city["name"]?.ToString();
            if (string.IsNullOrEmpty(cityName))
            {
                _logger.LogWarning("City name is null or empty, skipping.");
                continue;
            }

            _logger.LogInformation($"Processing city: {cityName}");

            var cityEntity = new City
            {
                Name = cityName,
                CountryId = countryId
            };

            _context.Cities.Add(cityEntity);
            await _context.SaveChangesAsync();
        }
    }
}









