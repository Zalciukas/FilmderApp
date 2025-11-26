using System.ComponentModel.DataAnnotations;

namespace Filmder.Models;

public class MovieRatingGuess
{
    public int Id { get; set; }
    
    [Required]
    public required string UserId { get; set; }
    
    public AppUser User { get; set; }
    
    [Required]
    public int MovieId { get; set; } 
    
    public Movie Movie { get; set; }
    
    [Required]
    [Range(0, 10)]
    public double RatingGuessValue { get; set; }
    
    public int? GuessRatingGameId { get; set; }
    
    public GuessRatingGame? GuessRatingGame { get; set; }
}