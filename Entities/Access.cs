using melodia.Entities;

namespace melodia_api.Entities;

public class Access
{
    public long Id { get; set; }
    public bool Insert { get; set; } 
    public bool Read { get; set; }
    public bool Update { get; set; }
    public bool Delete { get; set; }

    public Role Role { get; set; }
    public string RoleId { get; set; }

    public Section Section { get; set; }
    public long SectionId { get; set; }
}