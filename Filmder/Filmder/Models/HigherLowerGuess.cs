using System.ComponentModel.DataAnnotations;

namespace Filmder.Models;

public class HigherLowerGuess
{
    public int Id { get; set; }
    
    [Required]
    public int GameId { get; set; }
    public HigherLowerGame? Game { get; set; }
    
    [Required]
    public int Movie1Id { get; set; }
    public Movie? Movie1 { get; set; }
    
    [Required]
    public int Movie2Id { get; set; }
    public Movie? Movie2 { get; set; }
    
    [Required]
    public required string GuessedHigher { get; set; } // "higher" or "lower"
    
    public bool WasCorrect { get; set; }
    
    public DateTime GuessedAt { get; set; } = DateTime.UtcNow;
}