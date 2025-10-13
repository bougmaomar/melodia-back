namespace melodia_api.Models.Access;

public class AccessUpdateDto
{
    public long Id { get; set; }
    public bool Insert { get; set; }
    public bool Read { get; set; }
    public bool Update { get; set; }
    public bool Delete { get; set; }
    public string RoleId { get; set; }
    public long SectionId { get; set; }
}