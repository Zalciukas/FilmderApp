using System.ComponentModel.DataAnnotations;

namespace Filmder.Models;

public class Game
{  
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;
    public int GroupId { get; set; }
    
    public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
    public ICollection<Movie> Movies { get; set; } = new List<Movie>();
    public ICollection<MovieScore> MovieScores { get; set; } = new List<MovieScore>();
}