using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using melodia.Entities;

namespace melodia_api.Entities;

[Table("songvisits")]
public class SongVisit
{ 
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long SongId { get; set; }
        public Song Song { get; set; }

        public long RadioStationId { get; set; }
        public RadioStation RadioStation { get; set; }
        public int VisitNumber { get; set; }
    
}