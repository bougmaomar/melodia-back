using System.ComponentModel.DataAnnotations;

namespace melodia_api.Models.SongPOwner
{
    public class SongPOwnerViewDto
    {
        public long Id { get; set; }
        public long SongId { get; set; }
        public string SongName { get; set; }
        public long POwnerId { get; set; }
        public string POwnerName { get; set; }
    }
}
