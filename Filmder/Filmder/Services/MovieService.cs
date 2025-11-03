using Filmder.DTOs;
using Filmder.Extensions;
using Filmder.Models;
using Filmder.Repositories;

namespace Filmder.Services;

public class MovieService(IMovieRepository movies) : IMovieService
{
    public Task<List<Movie>> GetAllAsync(int page, int pageSize) => movies.GetPagedAsync(page, pageSize);

    public Task<Movie?> GetByIdAsync(int id) => movies.GetByIdAsync(id);

    public Task<List<Movie>> SearchAsync(string query) => movies.SearchAsync(query);

    public async Task<List<Movie>> GetByGenreAsync(string genre, int page, int pageSize)
    {
        if (!MovieGenreParsingExtensions.TryParseGenre(genre, out var parsed))
            throw new ArgumentException("Invalid genre");
        return await movies.GetByGenreAsync(parsed, page, pageSize);
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

        await movies.AddAsync(movie);
        await movies.SaveChangesAsync();
        return movie;
    }

    public async Task<bool> UpdateAsync(int id, CreateMovieDto dto)
    {
        var existing = await movies.GetByIdAsync(id);
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

        await movies.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await movies.GetByIdAsync(id);
        if (existing == null) return false;
        await movies.RemoveAsync(existing);
        await movies.SaveChangesAsync();
        return true;
    }
}


