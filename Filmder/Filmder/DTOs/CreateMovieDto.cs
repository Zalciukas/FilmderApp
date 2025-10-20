using System.ComponentModel.DataAnnotations;

namespace Filmder.DTOs;

public class CreateMovieDto
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Genre { get; set; } = string.Empty;

    [StringLength(2055)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(1900, 2026)]
    public int ReleaseYear { get; set; }

    [Range(0.0, 10.0)]
    public double Rating { get; set; }

    [Url]
    public string? PosterUrl { get; set; }

    [Url]
    public string? TrailerUrl { get; set; }

    [Required]
    [Range(1, 51420)]
    public int Duration { get; set; }

    [Required]
    public string Director { get; set; } = string.Empty;

    [StringLength(500)]
    public string Cast { get; set; } = string.Empty;        //CIA MANAU LISTAS TURETU BUTI??? BET MOVIES MODELYJE STRINGAS TODO
}