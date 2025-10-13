namespace melodia_api.Models;

public class CityViewDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string CountryName { get; set; }
    public CountryViewDto Country { get; set; }
}