namespace Filmder.DTOs.HigherLower;

public class MovieBasicDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Genre { get; set; }
    public int ReleaseYear { get; set; }
    public double? Rating { get; set; }
    public string? PosterUrl { get; set; }
}