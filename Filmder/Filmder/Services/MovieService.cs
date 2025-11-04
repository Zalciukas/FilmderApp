using Filmder.Data;
using Filmder.DTOs;
using Filmder.Extensions;
using Filmder.Models;
using Microsoft.EntityFrameworkCore;

namespace Filmder.Services;

public class MovieService(AppDbContext context) : IMovieService
{
    public async Task<List<Movie>> GetAllAsync(int page, int pageSize)
    {
        return await context.Movies
            .OrderByDescending(m => m.Rating)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Movie?> GetByIdAsync(int id)
    {
        return await context.Movies.FindAsync(id);
    }

    public async Task<List<Movie>> SearchAsync(string query)
    {
        return await context.Movies
            .Where(m => m.Name.Contains(query) || m.Director.Contains(query) || m.Cast.Contains(query))
            .OrderByDescending(m => m.Rating)
            .Take(50)
            .ToListAsync();
    }

    public async Task<List<Movie>> GetByGenreAsync(string genre, int page, int pageSize)
    {
        if (!MovieGenreParsingExtensions.TryParseGenre(genre, out var parsed))
            throw new ArgumentException("Invalid genre");
        
        return await context.Movies
            .Where(m => m.Genre == parsed)
            .OrderByDescending(m => m.Rating)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Movie> CreateAsync(CreateMovieDto dto)
    {
        if (!MovieGenreParsingExtensions.TryParseGenre(dto.Genre, out var createParsed))
            throw new ArgumentException("Invalid genre");

        var movie = new Movie
        {
            Name = dto.Name,
            Genre = createParsed,
            Description = dto.Description,
            ReleaseYear = dto.ReleaseYear,
            Rating = dto.Rating,
            PosterUrl = dto.PosterUrl,
            TrailerUrl = dto.TrailerUrl,
            Duration = dto.Duration,
            Director = dto.Director,
            Cast = dto.Cast
        };

        await context.Movies.AddAsync(movie);
        await context.SaveChangesAsync();
        return movie;
    }

    public async Task<bool> UpdateAsync(int id, CreateMovieDto dto)
    {
        var existing = await context.Movies.FindAsync(id);
        if (existing == null) return false;
        
        if (!MovieGenreParsingExtensions.TryParseGenre(dto.Genre, out var updateParsed))
            throw new ArgumentException("Invalid genre");

        existing.Name = dto.Name;
        existing.Genre = updateParsed;
        existing.Description = dto.Description;
        existing.ReleaseYear = dto.ReleaseYear;
        existing.Rating = dto.Rating;
        existing.PosterUrl = dto.PosterUrl;
        existing.TrailerUrl = dto.TrailerUrl;
        existing.Duration = dto.Duration;
        existing.Director = dto.Director;
        existing.Cast = dto.Cast;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await context.Movies.FindAsync(id);
        if (existing == null) return false;
        
        context.Movies.Remove(existing);
        await context.SaveChangesAsync();
        return true;
    }
}


