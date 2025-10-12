using System.ComponentModel.DataAnnotations;

namespace Filmder.DTOs;

public class RateMovieDto
{
    [Required]
    public int MovieId { get; set; }
    
    [Required]
    [Range(1, 10)]
    public int Score { get; set; }
    
    [StringLength(500)]
    public string? Comment { get; set; }
}