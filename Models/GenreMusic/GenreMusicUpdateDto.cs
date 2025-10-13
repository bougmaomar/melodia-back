using System.ComponentModel.DataAnnotations;

namespace melodia_api.Models.GenreMusic;

public class GenreMusicUpdateDto
{
    public int Id { get; set; } 
    [Required]  public string Name { get; set; }
}