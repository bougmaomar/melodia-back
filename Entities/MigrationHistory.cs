using System.ComponentModel.DataAnnotations;

namespace melodia_api.Entities;

public class MigrationHistory
{
    [Key]
    public string MigrationId { get; set; }
    public DateTime AppliedOn { get; set; }
}