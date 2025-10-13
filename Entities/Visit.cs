using melodia.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace melodia_api.Entities
{
    [Table("visits")]
    public class Visit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long ArtistId { get; set; }
        public Artist Artist { get; set; }

        public DateTime VisitDate { get; set; } = DateTime.UtcNow;

        public long RadioStationId { get; set; }
        public RadioStation RadioStation { get; set; }
    }
}
