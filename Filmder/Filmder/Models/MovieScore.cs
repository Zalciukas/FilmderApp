using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Filmder.Models;

public class MovieScore
{
    public int Id { get; set; }

    [Required]
    [ForeignKey("Movie")]
    public int MovieId { get; set; }

    public Movie? Movie { get; set; }

    [Required]
    [ForeignKey("Game")]
    public int GameId { get; set; }

    public Game? Game { get; set; }
    
    public int MovieScoreValue { get; set; }
}