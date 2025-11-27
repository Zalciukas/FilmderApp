using System.ComponentModel.DataAnnotations;

namespace Filmder.Models;

public class PersonalityMatchResult
{
    public int Id { get; set; }
    
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    public AppUser User { get; set; } = null!;
    
    [Required]
    [StringLength(200)]
    public string CharacterName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(200)]
    public string MovieOrSeries { get; set; } = string.Empty;
    
    [Range(0, 100)]
    public int MatchPercentage { get; set; }
    
    [StringLength(1000)]
    public string Explanation { get; set; } = string.Empty;
    
    public string? ImageUrl { get; set; }
    
    [StringLength(1000)]
    public string PersonalityProfile { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}