using Filmder.Data;
using Filmder.Models;
using Microsoft.EntityFrameworkCore;

namespace Filmder.Repositories;

public class MovieRepository(AppDbContext context) : IMovieRepository
{
    public Task<List<Movie>> GetPagedAsync(int page, int pageSize)
    {
        return context.Movies
            .OrderByDescending(m => m.Rating)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public Task<Movie?> GetByIdAsync(int id)
    {
        return context.Movies.FindAsync(id).AsTask();
    }

    public Task<List<Movie>> SearchAsync(string query)
    {
        return context.Movies
            .Where(m => m.Name.Contains(query) || m.Director.Contains(query) || m.Cast.Contains(query))
            .OrderByDescending(m => m.Rating)
            .Take(50)
            .ToListAsync();
    }

    public Task<List<Movie>> GetByGenreAsync(MovieGenre genre, int page, int pageSize)
    {
        return context.Movies
            .Where(m => m.Genre == genre)
            .OrderByDescending(m => m.Rating)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task AddAsync(Movie movie)
    {
        await context.Movies.AddAsync(movie);
    }

    public Task RemoveAsync(Movie movie)
    {
        context.Movies.Remove(movie);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync()
    {
        return context.SaveChangesAsync();
    }
}


