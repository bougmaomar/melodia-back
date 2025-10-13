using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace melodia.Entities
{
    public class Programme
    {
        [Key] 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public long Id { get; set; } 
        public string Title { get; set; } 
        public string Description { get; set; } 
        public DateTime Duration { get; set; } 
        public DateTime Schedule {  get; set; } 
        public bool Active { get; set; } = true;
        
        public ProgramType ProgramType { get; set; } 
        public long ProgramTypeId { get; set; }
        
        public RadioStation RadioStation { get; set; }
        public long RadioStationId { get; set; }
    }
}
