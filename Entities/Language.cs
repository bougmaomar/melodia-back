using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace melodia.Entities
{
    [Table("languages")]
    public class Language
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; } 
        [Required] public string Label { get; set; }
        public bool Active { get; set; } = true;
        
        public List<Song> Songs { get; set; }
        public List<RadioStationLanguage> StationLanguages { get; set; }
    }
}
