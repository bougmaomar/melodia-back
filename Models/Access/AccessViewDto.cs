using melodia_api.Models.Role;
using melodia_api.Models.Section;

namespace melodia_api.Models.Access;

public class AccessViewDto
{
    public long Id { get; set; }
    public bool Insert { get; set; }
    public bool Read { get; set; }
    public bool Update { get; set; }
    public bool Delete { get; set; }
    public RoleViewDto Role { get; set; }
    public SectionViewDto Section { get; set; }
}