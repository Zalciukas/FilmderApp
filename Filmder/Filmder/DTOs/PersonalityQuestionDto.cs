namespace Filmder.DTOs;

public class PersonalityQuestionDto
{
    public int Id { get; set; }
    public string Question { get; set; } = string.Empty;
    public List<string> Options { get; set; } = new();
    public int OrderIndex { get; set; }
}