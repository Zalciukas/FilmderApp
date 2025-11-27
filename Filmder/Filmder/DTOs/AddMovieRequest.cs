namespace Filmder.DTOs;

public class AddMovieRequest
{
    public int MovieId { get; set; }
    public int? RatingScore { get; set; }
    public string? Comment { get; set; }
}
