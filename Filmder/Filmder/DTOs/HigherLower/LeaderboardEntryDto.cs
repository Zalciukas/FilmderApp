namespace Filmder.DTOs.HigherLower;

public class LeaderboardEntryDto
{
    public required string Username { get; set; }
    public int BestStreak { get; set; }
    public DateTime AchievedAt { get; set; }
}