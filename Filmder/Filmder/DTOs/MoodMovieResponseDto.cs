using Filmder.Models;

namespace Filmder.DTOs;

public class MoodMovieResponseDto
{
    public string Mood { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> RecommendedGenres { get; set; } = new List<string>();
    public Movie Movie { get; set; } = null!;
}