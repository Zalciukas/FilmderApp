using Filmder.DTOs;

namespace Filmder.Services;

public interface IRatingService
{
    Task SaveRatingAsync(string userId, RateMovieDto dto);
    Task<List<object>> GetRatingsAsync(int movieId);
    Task<(double averageScore, int totalRatings)> GetAverageAsync(int movieId);
}


