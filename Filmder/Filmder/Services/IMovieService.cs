using Filmder.DTOs;
using Filmder.Models;

namespace Filmder.Services;

public interface IMovieService
{
    Task<List<Movie>> GetAllAsync(int page, int pageSize);
    Task<Movie?> GetByIdAsync(int id);
    Task<List<Movie>> SearchAsync(string query);
    Task<List<Movie>> GetByGenreAsync(string genre, int page, int pageSize);
    Task<Movie> CreateAsync(CreateMovieDto dto);
    Task<bool> UpdateAsync(int id, CreateMovieDto dto);
    Task<bool> DeleteAsync(int id);
}


