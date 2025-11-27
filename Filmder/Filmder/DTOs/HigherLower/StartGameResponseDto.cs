namespace Filmder.DTOs.HigherLower;

public class StartGameResponseDto
{
    public int GameId { get; set; }
    public required MovieComparisonDto Comparison { get; set; }
    public int CurrentStreak { get; set; }
    public int BestStreak { get; set; }
}