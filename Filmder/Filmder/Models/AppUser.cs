using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Filmder.Models
{
    public class AppUser : IdentityUser
    {
        // IdentityUser already provides: Id, UserName, Email, PasswordHash, etc.
        
        [StringLength(50)]
        public string? FavoriteGenre { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public ICollection<Group> Groups { get; set; } = new List<Group>();
        /*  TODO
        // Navigation properties
        public ICollection<UserMovieLike> LikedMovies { get; set; } = new List<UserMovieLike>();
        public ICollection<UserMovieDislike> DislikedMovies { get; set; } = new List<UserMovieDislike>();
        public ICollection<LobbyUser> LobbyMemberships { get; set; } = new List<LobbyUser>();
        public ICollection<Lobby> CreatedLobbies { get; set; } = new List<Lobby>();
        */
    }
}