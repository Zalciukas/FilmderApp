using System.ComponentModel.DataAnnotations;

namespace Filmder.Models;

public class HigherLowerGame
{
    public int Id { get; set; }
    
    [Required]
    public required string UserId { get; set; }
    public AppUser? User { get; set; }
    
    public int CurrentStreak { get; set; } = 0;
    public int BestStreak { get; set; } = 0;
    
    public int? CurrentMovieId { get; set; }
    public Movie? CurrentMovie { get; set; }
    
    public int? NextMovieId { get; set; }
    public Movie? NextMovie { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EndedAt { get; set; }
    
    public ICollection<HigherLowerGuess> Guesses { get; set; } = new List<HigherLowerGuess>();
}