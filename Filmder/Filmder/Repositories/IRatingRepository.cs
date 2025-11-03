using Filmder.Models;

namespace Filmder.Repositories;

public interface IRatingRepository
{
    Task<Movie?> FindMovieAsync(int movieId);
    Task<Rating?> FindUserRatingAsync(string userId, int movieId);
    Task AddRatingAsync(Rating rating);
    Task SaveChangesAsync();
    Task<List<object>> GetMovieRatingsProjectionAsync(int movieId);
    Task<List<Rating>> GetMovieRatingsAsync(int movieId);
}


