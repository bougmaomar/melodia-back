using System.ComponentModel.DataAnnotations;

namespace melodia_api.Models.SongPOwner
{
    public class SongPOwnerUpdateDto
    {
        public long Id { get; set; }
        [Required] public long SongId { get; set; }
        [Required] public long POwnerId { get; set; }
    }
}
