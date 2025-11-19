using System.ComponentModel.DataAnnotations;

namespace Filmder.DTOs;

public class RatingGuessDto
{
    [Required]
    [Range(0, 10)]
    public double RatingGuessValue { get; set; }
}