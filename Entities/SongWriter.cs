using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace melodia.Entities;

[Table("songwritters")]
public class SongWriter
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public long SongId { get; set; }
    public Song Song { get; set; }

    public long WriterId { get; set; }
    public Writer Writer { get; set; }
    
    public bool Active { get; set; } = true;
}