namespace melodia.Entities;

public class RadioStationMusicFormat
{
    public RadioStation RadioStation { get; set; }
    public long RadioStationId { get; set; }
    
    public MusicFormat MusicFormat { get; set; }
    public long MusicFormatId { get; set; }
}