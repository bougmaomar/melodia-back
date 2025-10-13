using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using melodia.Entities;

namespace melodia.Entities
{
    public class CROwner
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]  public string Name { get; set; }
        public bool Active { get; set; } = true;
        public List<SongCROwner> SongCrOwners { get; set; }
    }
}
