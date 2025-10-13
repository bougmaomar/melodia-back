using System.ComponentModel.DataAnnotations;

namespace melodia_api.Models.SongPOwner
{
    public class SongPOwnerCreateDto
    {
        [Required] public long SongId { get; set; }
        [Required] public long POwnerId { get; set; }
    }
}
