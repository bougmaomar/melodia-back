using melodia.Entities;

namespace melodia_api.Entities;

public class FavoriteSongs
{
    public string UserId { get; set; }
    
    public virtual Account Account { get; set; }
    
    public long SongId { get; set; }
    
    public virtual Song Song { get; set; }
    
}