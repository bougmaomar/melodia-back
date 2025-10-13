namespace melodia.Entities;

public class RadioStationLanguage
{
    public RadioStation RadioStation { get; set; }
    public long RadioStationId { get; set; }
    
    public Language Language { get; set; }
    public long LanguageId { get; set; }
}