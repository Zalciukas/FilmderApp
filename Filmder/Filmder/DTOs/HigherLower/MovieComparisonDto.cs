namespace Filmder.DTOs.HigherLower;

public class MovieComparisonDto
{
    public required MovieBasicDto CurrentMovie { get; set; }
    public required MovieBasicDto NextMovie { get; set; }
}