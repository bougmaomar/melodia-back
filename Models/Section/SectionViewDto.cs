namespace melodia_api.Models.Section;

public class SectionViewDto
{
    public long Id { get; set; }
    public string Label { get; set; }
    public long? ParentSectionId { get; set; }
    public List<SectionViewDto> SubSections { get; set; }
}