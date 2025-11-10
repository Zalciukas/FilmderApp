namespace Filmder.Models;

public struct MovieDuration
{
    public int Hours { get; }
    public int Minutes { get; }
    public int TotalMinutes { get; }
    
    public MovieDuration(int totalMinutes)
    {
        TotalMinutes = totalMinutes;
        Hours = totalMinutes / 60;
        Minutes = totalMinutes % 60;
    }
    
    public override string ToString() => $"{Hours}h {Minutes}m";
}