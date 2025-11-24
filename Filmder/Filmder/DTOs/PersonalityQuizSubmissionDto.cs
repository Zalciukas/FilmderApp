using System.ComponentModel.DataAnnotations;

namespace Filmder.DTOs;

public class PersonalityQuizSubmissionDto
{
    [Required]
    [MinLength(1, ErrorMessage = "At least one answer is required")]
    public List<PersonalityAnswerDto> Answers { get; set; } = new();
}