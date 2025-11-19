using System.Collections.Concurrent;
using Filmder.Data;
using Filmder.Models;
using Microsoft.EntityFrameworkCore;

namespace Filmder.Services;

public interface IMovieCacheService
{
    Task<Movie?> GetMovieByIdAsync(int movieId);
    void InvalidateCache(int movieId);
    void ClearCache();
}

public class MovieCacheService : IMovieCacheService
{
    private readonly AppDbContext _context;
    private readonly ConcurrentDictionary<int, Movie> _movieCache;

    public MovieCacheService(AppDbContext context)
    {
        _context = context;
        _movieCache = new ConcurrentDictionary<int, Movie>();
    }

    public async Task<Movie?> GetMovieByIdAsync(int movieId)
    {
        if (_movieCache.TryGetValue(movieId, out var cachedMovie))
        {
            return cachedMovie;
        }

        var movie = await _context.Movies
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == movieId);

        if (movie != null)
        {
            _movieCache.TryAdd(movieId, movie);
        }

        return movie;
    }

    public void InvalidateCache(int movieId)
    {
        _movieCache.TryRemove(movieId, out _);
    }

    public void ClearCache()
    {
        _movieCache.Clear();
    }
}