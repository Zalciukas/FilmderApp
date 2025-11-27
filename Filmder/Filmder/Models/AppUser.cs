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
        
        public string? ProfilePictureUrl { get; set; }

        public ICollection<UserMovie> UserMovies { get; set; } = new List<UserMovie>();

        
       // public ICollection<Group> Groups { get; set; } = new List<Group>();
    }
}