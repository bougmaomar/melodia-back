using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace melodia.Entities
{
    [Table("employees")] 
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; } 
        [Required] public string FirstName { get; set; } 
        [Required] public string LastName { get; set; } 
        [Required] public char Sexe { get; set; } 
        public DateTime HiringDate { get; set; } 
        public DateTime? DepartureDate { get; set; }
        
        public Position Position { get; set; }
        public long PositionId { get; set; }
        
        public RadioStation RadioStation { get; set; }
        public long? RadioStationId { get; set; }

        public bool Active { get; set; } = true;
    }
}
