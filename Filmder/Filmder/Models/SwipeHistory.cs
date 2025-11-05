using System.ComponentModel.DataAnnotations;

namespace Filmder.Models;

public class SwipeHistory
{
    public int Id { get; set; }
    
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    [Required]
    public int MovieId { get; set; }
    
    [Required]
    public bool IsLike { get; set; }
    
    public DateTime SwipedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public AppUser User { get; set; } = null!;
    public Movie Movie { get; set; } = null!;
}