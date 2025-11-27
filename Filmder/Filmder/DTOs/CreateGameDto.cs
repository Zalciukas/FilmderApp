using System.ComponentModel.DataAnnotations;
using Filmder.Models;

namespace Filmder.DTOs;

public class CreateGameDto
{
    [Required]
    [StringLength(50)]
    public required string Name { get; set; }
    
    public List<string> UserEmails { get; set; }
    [Required]
    public int GroupId { get; set; }

    public List<Movie> Movies { get; set; } = new();
    public List<MovieScore> MovieScores { get; set; } = new();
}