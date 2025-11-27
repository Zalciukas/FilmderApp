namespace Filmder.DTOs.HigherLower;

public class GuessResultDto
{
    public bool WasCorrect { get; set; }
    public double ActualRating { get; set; }
    public int CurrentStreak { get; set; }
    public int BestStreak { get; set; }
    public bool GameOver { get; set; }
    public MovieComparisonDto? NextComparison { get; set; }
}