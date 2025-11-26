namespace Filmder.DTOs.HigherLower;

public class HigherLowerStatsDto
{
    public int TotalGames { get; set; }
    public int BestStreak { get; set; }
    public int TotalCorrectGuesses { get; set; }
    public int TotalGuesses { get; set; }
    public double AccuracyPercentage { get; set; }
}