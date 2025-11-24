using System.ComponentModel.DataAnnotations;

namespace Filmder.Models;

public class PersonalityAnswer
{
    public int Id { get; set; }
    
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    public AppUser User { get; set; } = null!;
    
    [Required]
    public int QuestionId { get; set; }
    
    public PersonalityQuestion Question { get; set; } = null!;
    
    [Required]
    [StringLength(500)]
    public string Answer { get; set; } = string.Empty;
    
    public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;
}