namespace Filmder.DTOs;

public class WatchlistMovieDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public double Rating { get; set; }
    public string PosterUrl { get; set; } = string.Empty;
    public string TrailerUrl { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string Director { get; set; } = string.Empty;
    public string Cast { get; set; } = string.Empty;
    public double RecommendationScore { get; set; }
}

public class UserPreferencesDto
{
    public string FavoriteGenre { get; set; } = string.Empty;
    public Dictionary<string, double> GenreScores { get; set; } = new();
    public int TotalRatings { get; set; }
    public int TotalGameVotes { get; set; }
}