using Filmder.DTOs;
using Filmder.Models;
using Filmder.Repositories;

namespace Filmder.Services;

public class GameService(IGameRepository games) : IGameService
{
    public async Task<Game> CreateAsync(CreateGameDto dto)
    {
        var users = await games.GetUsersByEmailsAsync(dto.UserEmails);
        var game = new Game
        {
            Name = dto.name,
            Users = users,
            GroupId = dto.groupId,
            Movies = dto.Movies,
            MovieScores = dto.MovieScores
        };
        await games.AddAsync(game);
        await games.SaveChangesAsync();
        return game;
    }

    public async Task VoteAsync(VoteDto voteDto)
    {
        var game = await games.GetWithScoresAsync(voteDto.GameId) ?? throw new ArgumentException("Bad game");
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
        await games.SaveChangesAsync();
    }

    public Task<List<Movie>> GetMoviesByCriteriaAsync(string? genre, int? releaseDate, int? longestDurationMinutes, int movieCount)
    {
        return games.GetMoviesByCriteriaAsync(genre, releaseDate, longestDurationMinutes, movieCount);
    }

    public async Task<List<Movie>> GetResultsAsync(int gameId)
    {
        var game = await games.FindByIdAsync(gameId) ?? throw new ArgumentException("Bad game");
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
        var game = await games.FindByIdAsync(gameId);
        if (game == null) return false;
        game.IsActive = false;
        await games.SaveChangesAsync();
        return true;
    }

    public Task<List<Game>> GetActiveGamesAsync(int groupId)
    {
        return games.GetActiveByGroupAsync(groupId);
    }
}


