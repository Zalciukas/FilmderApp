using System.ComponentModel.DataAnnotations;

namespace Filmder.Models;


public class UserMovie
{
    [Required]
    public string UserId { get; set; } = string.Empty;
    public AppUser User { get; set; } = null!;

    [Required]
    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;

    public DateTime WatchedAt { get; set; } = DateTime.UtcNow;

    public int? RatingId { get; set; }
    public Rating? Rating { get; set; }
}
