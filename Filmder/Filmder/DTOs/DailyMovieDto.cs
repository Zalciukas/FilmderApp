namespace Filmder.DTOs
{
    public class DailyMovieDto
    {
        public string Name { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PosterUrl { get; set; } = string.Empty;
        public string TrailerUrl { get; set; } = string.Empty;
        public double Rating { get; set; }
        public int ReleaseYear { get; set; }
        public int CountdownSeconds { get; set; }
        public DateTime NextUpdate { get; set; }
    }
}