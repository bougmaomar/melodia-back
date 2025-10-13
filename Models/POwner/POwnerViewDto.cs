using melodia.Entities;
using System.ComponentModel.DataAnnotations;

namespace melodia_api.Models.POwner
{
    public class POwnerViewDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
    }
}
