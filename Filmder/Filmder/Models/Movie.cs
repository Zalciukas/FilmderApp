using System.ComponentModel.DataAnnotations;

namespace Filmder.Models
{
    public class Movie
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Genre { get; set; } = string.Empty;
        
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
        
        /*  TODO
        // Navigation properties for many-to-many relationships
        public ICollection<UserMovieLike> UserLikes { get; set; } = new List<UserMovieLike>();
        public ICollection<UserMovieDislike> UserDislikes { get; set; } = new List<UserMovieDislike>();
        public ICollection<LobbyMovie> LobbyMovies { get; set; } = new List<LobbyMovie>();
        */
    }
}