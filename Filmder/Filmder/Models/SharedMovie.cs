using System.ComponentModel.DataAnnotations;

namespace Filmder.Models;

public class SharedMovie
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public string UserWhoAddedId { get; set; }
    public string UserId { get; set; }  
    public AppUser? User { get; set; }
    public int MovieId { get; set; }
    public Movie? Movie { get; set; }
    [StringLength(200)]
    public string? Comment { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}