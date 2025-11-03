using Filmder.DTOs;
using Filmder.Models;

namespace Filmder.Repositories;

public interface IGameRepository
{
    Task AddAsync(Game game);
    Task<Game?> GetWithScoresAsync(int gameId);
    Task<Game?> FindByIdAsync(int gameId);
    Task<List<Game>> GetActiveByGroupAsync(int groupId);
    Task<List<Movie>> GetMoviesByCriteriaAsync(string? genre, int? releaseDate, int? longestDurationMinutes, int movieCount);
    Task<List<AppUser>> GetUsersByEmailsAsync(IEnumerable<string> emails);
    Task SaveChangesAsync();
}


