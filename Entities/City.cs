using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace melodia.Entities;

[Table("cities")]
public class City
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    public string Name { get; set; }

    [ForeignKey("Country")]
    public long CountryId { get; set; }
    public Country Country { get; set; }
}