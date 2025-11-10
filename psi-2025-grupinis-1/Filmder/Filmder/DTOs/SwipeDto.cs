namespace Filmder.DTOs;

public class SwipeDto
{
    public int MovieId { get; set; }
    public bool IsLike { get; set; } // true for like, false for dislike
}

public class SwipeHistoryDto
{
    public int Id { get; set; }
    public int MovieId { get; set; }
    public string MovieName { get; set; } = string.Empty;
    public string PosterUrl { get; set; } = string.Empty;
    public bool IsLike { get; set; }
    public DateTime SwipedAt { get; set; }
}