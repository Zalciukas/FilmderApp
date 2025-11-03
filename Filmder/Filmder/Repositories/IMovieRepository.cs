using Filmder.DTOs;
using Filmder.Models;

namespace Filmder.Repositories;

public interface IMovieRepository
{
    Task<List<Movie>> GetPagedAsync(int page, int pageSize);
    Task<Movie?> GetByIdAsync(int id);
    Task<List<Movie>> SearchAsync(string query);
    Task<List<Movie>> GetByGenreAsync(MovieGenre genre, int page, int pageSize);
    Task AddAsync(Movie movie);
    Task RemoveAsync(Movie movie);
    Task SaveChangesAsync();
}


