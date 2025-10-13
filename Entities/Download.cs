using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using melodia.Entities;

namespace melodia_api.Entities;

[Table("downloads")]
public class Download
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    
    public long SongId { get; set; }
    public Song Song { get; set; }
    
    public DateTime DownloadDate { get; set; } = DateTime.UtcNow;
    
    public long RadioStationId { get; set; }
    public RadioStation RadioStation { get; set; }
}