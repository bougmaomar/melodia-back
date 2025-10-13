using melodia.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace melodia.Entities
{
    [Table("composers")]

    public class Composer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required] public string Name { get; set; }
        public List<SongComposer> SongComposers { get; set; }
        public bool Active { get; set; } = true;
    }
}
