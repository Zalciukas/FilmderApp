namespace Filmder.DTOs;

public class PagedMoviesResponseDto
{
    public int Page { get; set; }
    public int TotalPages { get; set; }
    public List<SimpleMovieDto> Movies { get; set; }
}
