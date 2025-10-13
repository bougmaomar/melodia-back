using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DocumentFormat.OpenXml.Office2010.ExcelAc;

namespace melodia.Entities
{
    public class StationType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; } = true;
        
        public List<RadioStation> RadioStations { get; set; }

    }
}
