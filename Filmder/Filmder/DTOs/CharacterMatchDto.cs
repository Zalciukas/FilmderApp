namespace Filmder.DTOs;

public class CharacterMatchDto
{
    public string CharacterName { get; set; } = string.Empty;
    public string MovieOrSeries { get; set; } = string.Empty;
    public int MatchPercentage { get; set; }
    public string Explanation { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}