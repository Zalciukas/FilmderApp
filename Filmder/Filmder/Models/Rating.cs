using System.ComponentModel.DataAnnotations;

namespace Filmder.Models;

public class Rating
{
    public int Id { get; set; }
    
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    [Required]
    public int MovieId { get; set; }
    
    [Required]
    [Range(1, 10)]
    public int Score { get; set; }
    
    [StringLength(500)]
    public string? Comment { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public AppUser User { get; set; } = null!;
    public Movie Movie { get; set; } = null!;
}