using System.ComponentModel.DataAnnotations;

namespace Filmder.Models;

public class GroupMember
{
    public int Id { get; set; }
    
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    [Required]
    public int GroupId { get; set; }
    
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public AppUser User { get; set; } = null!;
    public Group Group { get; set; } = null!;
}