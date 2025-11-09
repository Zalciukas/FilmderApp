namespace Filmder.Models;

public class GuessRatingGame
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public Group Group { get; set; }
    public String UserId { get; set; }
    public AppUser User { get; set; }
    public bool IsActive { get; set; } = true;

    public List<Movie> Movies { get; set; }
    public List<MovieRatingGuess> Guesses { get; set; }
    
}