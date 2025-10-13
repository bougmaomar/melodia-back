using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace melodia.Entities
{
    public class MusicFormat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Name { get; set; }
        public List<RadioStationMusicFormat> StationMusicFormats { get; set; }
        public bool Active { get; set; } = true;

    }
}
