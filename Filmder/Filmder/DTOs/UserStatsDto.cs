namespace Filmder.DTOs;

public class UserStatsDto
{
    public int TotalMoviesWatched { get; set; }
    public int TotalRatings { get; set; }
    public double? AverageRating { get; set; }

    public List<string> TopGenres { get; set; } = new();

    public List<FavoriteMovieDto> FavoriteMovies { get; set; } = new();
}
