using Filmder.Models;

namespace Filmder.DTOs;

public class CreateGameDto
{
    public String name { get; set; }
    public List<String> UserEmails { get; set; }
    public int groupId { get; set; }
    public List<Movie> Movies { get; set; }
    public List<MovieScore> MovieScores { get; set; }
}