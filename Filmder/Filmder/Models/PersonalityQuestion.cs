using System.ComponentModel.DataAnnotations;

namespace Filmder.Models;

public class PersonalityQuestion
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(500)]
    public string Question { get; set; } = string.Empty;
    
    [Required]
    public string Options { get; set; } = string.Empty;
    
    public int OrderIndex { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<PersonalityAnswer> Answers { get; set; } = new List<PersonalityAnswer>();
}