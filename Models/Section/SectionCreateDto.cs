namespace melodia_api.Models.Section;

public class SectionCreateDto
{
    public string Label { get; set; }
    public long? ParentSectionId { get; set; }
}