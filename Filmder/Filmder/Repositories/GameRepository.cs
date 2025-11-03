using Filmder.Data;
using Filmder.Extensions;
using Filmder.Models;
using Microsoft.EntityFrameworkCore;

namespace Filmder.Repositories;

public class GameRepository(AppDbContext context) : IGameRepository
{
    public async Task AddAsync(Game game)
    {
        await context.Games.AddAsync(game);
    }

    public Task<Game?> GetWithScoresAsync(int gameId)
    {
        return context.Games
            .Include(g => g.MovieScores)
            .FirstOrDefaultAsync(g => g.Id == gameId);
    }

    public Task<Game?> FindByIdAsync(int gameId)
    {
        return context.Games.FindAsync(gameId).AsTask();
    }

    public Task<List<Game>> GetActiveByGroupAsync(int groupId)
    {
        return context.Games
            .Include(g => g.Users)
            .Where(mv => mv.IsActive && mv.GroupId == groupId)
            .ToListAsync();
    }

    public async Task<List<Movie>> GetMoviesByCriteriaAsync(string? genre, int? releaseDate, int? longestDurationMinutes, int movieCount)
    {
        var movies = context.Movies.AsQueryable();

        if (longestDurationMinutes.HasValue)
            movies = movies.Where(mv => mv.Duration <= longestDurationMinutes.Value);

        if (releaseDate.HasValue)
            movies = movies.Where(mv => mv.ReleaseYear >= releaseDate.Value);

        if (!string.IsNullOrEmpty(genre))
        {
            if (MovieGenreParsingExtensions.TryParseGenre(genre, out var parsedGenre))
                movies = movies.Where(mv => mv.Genre == parsedGenre);
            else
                throw new ArgumentException("Invalid genre");
        }

        return await movies.Take(movieCount).ToListAsync();
    }

    public Task<List<AppUser>> GetUsersByEmailsAsync(IEnumerable<string> emails)
    {
        return context.Users.Where(usr => emails.Contains(usr.Email!)).ToListAsync();
    }

    public Task SaveChangesAsync()
    {
        return context.SaveChangesAsync();
    }
}


