using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using melodia.Entities;

namespace melodia.Entities
{
    [Table("writers")]
    public class Writer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required] public string Name { get; set; }
        public bool Active { get; set; } = true;
        public List<SongWriter> SongWriters { get; set; }
    }
}
