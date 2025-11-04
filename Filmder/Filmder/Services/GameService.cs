using Filmder.Data;
using Filmder.DTOs;
using Filmder.Extensions;
using Filmder.Models;
using Microsoft.EntityFrameworkCore;

namespace Filmder.Services;

public class GameService(AppDbContext context) : IGameService
{
    public async Task<Game> CreateAsync(CreateGameDto dto)
    {
        var users = await context.Users
            .Where(usr => dto.UserEmails.Contains(usr.Email!))
            .ToListAsync();
        
        var game = new Game
        {
            Name = dto.name,
            Users = users,
            GroupId = dto.groupId,
            Movies = dto.Movies,
            MovieScores = dto.MovieScores
        };
        
        await context.Games.AddAsync(game);
        await context.SaveChangesAsync();
        return game;
    }

    public async Task VoteAsync(VoteDto voteDto)
    {
        var game = await context.Games
            .Include(g => g.MovieScores)
            .FirstOrDefaultAsync(g => g.Id == voteDto.GameId);
        
        if (game == null) throw new ArgumentException("Bad game");
        
        var movieScore = game.MovieScores.FirstOrDefault(ms => ms.MovieId == voteDto.MovieId);
        if (movieScore == null)
        {
            movieScore = new MovieScore
            {
                MovieId = voteDto.MovieId,
                GameId = voteDto.GameId,
                MovieScoreValue = voteDto.Score
            };
            game.MovieScores.Add(movieScore);
        }
        else
        {
            movieScore.MovieScoreValue += voteDto.Score;
        }
        
        await context.SaveChangesAsync();
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

    public async Task<List<Movie>> GetResultsAsync(int gameId)
    {
        var game = await context.Games
            .Include(g => g.MovieScores)
            .Include(g => g.Movies)
            .FirstOrDefaultAsync(g => g.Id == gameId);
        
        if (game == null) throw new ArgumentException("Bad game");
        
        var topMovies = game.MovieScores
            .Take(5)
            .Select(ms => game.Movies.FirstOrDefault(m => m.Id == ms.MovieId))
            .Where(m => m != null)
            .Cast<Movie>()
            .ToList();
        
        topMovies.Sort();
        return topMovies;
    }

    public async Task<bool> EndGameAsync(int gameId)
    {
        var game = await context.Games.FindAsync(gameId);
        if (game == null) return false;
        
        game.IsActive = false;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Game>> GetActiveGamesAsync(int groupId)
    {
        return await context.Games
            .Include(g => g.Users)
            .Where(mv => mv.IsActive && mv.GroupId == groupId)
            .ToListAsync();
    }
}


