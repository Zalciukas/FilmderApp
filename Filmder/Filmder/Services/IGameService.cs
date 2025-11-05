using Filmder.DTOs;
using Filmder.Models;

namespace Filmder.Services;

public interface IGameService
{
    Task<Game> CreateAsync(CreateGameDto dto);
    Task VoteAsync(VoteDto voteDto);
    Task<List<Movie>> GetMoviesByCriteriaAsync(string? genre, int? releaseDate, int? longestDurationMinutes, int movieCount);
    Task<List<Movie>> GetResultsAsync(int gameId);
    Task<bool> EndGameAsync(int gameId);
    Task<List<Game>> GetActiveGamesAsync(int groupId);
}


