using Filmder.Models;

namespace Filmder.DTOs;

public class CreateGameDto
{
    public String name { get; set; }
    public ICollection<AppUser> Users { get; set; }
    public ICollection<Movie> Movies { get; set; }
    public ICollection<MovieScore> MovieScores { get; set; }
}