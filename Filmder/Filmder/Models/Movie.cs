using System.ComponentModel.DataAnnotations;

namespace Filmder.Models
{
    public class Movie : IComparable<Movie>
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public MovieGenre Genre { get; set; }
        
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Range(1888, 2030)] // First movie made in 1888 (Roundhay Garden Scene) *knowledge acquired
        public int ReleaseYear { get; set; }

        [Range(0.0, 10.0)]
        public double Rating { get; set; }

        [Url]
        public string? PosterUrl { get; set; }

        [Url]
        public string? TrailerUrl { get; set; }

        public int Duration { get; set; } // In minutes

        public string Director { get; set; } = string.Empty;

        [StringLength(500)]
        public string Cast { get; set; } = string.Empty; // Main actors, comma-separated

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public MovieDuration GetFormattedDuration() => new MovieDuration(Duration);
        
        public ICollection<UserMovie> UserMovies { get; set; } = new List<UserMovie>();
        
        public int CompareTo(Movie? other)
        {
            if (other == null) return 1;
            
            
            int ratingComparison = other.Rating.CompareTo(this.Rating);
            if (ratingComparison != 0) return ratingComparison;

            // if ratings are equal, compare by release date
            return other.ReleaseYear.CompareTo(this.ReleaseYear);
        }
        
    }
}