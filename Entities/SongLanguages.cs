using melodia.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace melodia_api.Entities
{

    public class SongLanguages
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long SongId { get; set; }
        public Song Song { get; set; }

        public long LanguageId { get; set; }
        public Language Language { get; set; }

        public bool Active { get; set; } = true;
    }
}
