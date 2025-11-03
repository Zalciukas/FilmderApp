using Filmder.DTOs;
using Filmder.Models;
using Filmder.Repositories;

namespace Filmder.Services;

public class RatingService(IRatingRepository ratings) : IRatingService
{
    public async Task SaveRatingAsync(string userId, RateMovieDto dto)
    {
        var movie = await ratings.FindMovieAsync(dto.MovieId);
        if (movie == null) throw new KeyNotFoundException("Movie not found");

        var existing = await ratings.FindUserRatingAsync(userId, dto.MovieId);
        if (existing != null)
        {
            existing.Score = dto.Score;
            existing.Comment = dto.Comment;
            existing.CreatedAt = DateTime.UtcNow;
        }
        else
        {
            var rating = new Rating
            {
                UserId = userId,
                MovieId = dto.MovieId,
                Score = dto.Score,
                Comment = dto.Comment
            };
            await ratings.AddRatingAsync(rating);
        }

        await ratings.SaveChangesAsync();
    }

    public Task<List<object>> GetRatingsAsync(int movieId)
    {
        return ratings.GetMovieRatingsProjectionAsync(movieId);
    }

    public async Task<(double averageScore, int totalRatings)> GetAverageAsync(int movieId)
    {
        var list = await ratings.GetMovieRatingsAsync(movieId);
        if (!list.Any()) return (0, 0);
        var average = list.Average(r => r.Score);
        return (Math.Round(average, 1), list.Count);
    }
}


