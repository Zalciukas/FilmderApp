using System.ComponentModel.DataAnnotations;

namespace Filmder.DTOs;

public class FavoriteMovieDto
{
    public int MovieId { get; set; }
    [Required]
    public required string Title { get; set; }
    public int Score { get; set; }
    public string? PosterUrl { get; set; }
}
