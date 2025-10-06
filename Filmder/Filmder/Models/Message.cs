using System.ComponentModel.DataAnnotations;

namespace Filmder.Models;

public class Message
{
    public int Id { get; set; }
    
    [Required]
    public int GroupId { get; set; }
    
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    [Required]
    [StringLength(2000)]
    public string Content { get; set; } = string.Empty;
    
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public Group Group { get; set; } = null!;
    public AppUser User { get; set; } = null!;
}