using System.ComponentModel.DataAnnotations;

namespace Filmder.Models;

public class MovieRatingGuess
{
    public int Id { get; set; }
    public String UserId { get; set; }
    public AppUser User { get; set; }
    public int MovieId { get; set; } 
    public Movie movie { get; set; }
    public int RatingGuessValue { get; set; }


}