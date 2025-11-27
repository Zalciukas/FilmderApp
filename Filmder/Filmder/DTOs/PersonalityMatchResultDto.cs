namespace Filmder.DTOs;

public class PersonalityMatchResultDto
{
    public List<CharacterMatchDto> Matches { get; set; } = new();
    public string PersonalityProfile { get; set; } = string.Empty;
}