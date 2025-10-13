using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace melodia.Entities;

[Table("countries")]
public class Country
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    public string Name { get; set; }
    
    [Required]
    public string Code { get; set; }

    public List<City> Cities { get; set; } = new List<City>();
}