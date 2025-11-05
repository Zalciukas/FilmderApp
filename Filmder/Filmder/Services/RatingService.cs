using Filmder.Data;
using Filmder.DTOs;
using Filmder.Models;
using Microsoft.EntityFrameworkCore;

namespace Filmder.Services;

public class RatingService(AppDbContext context) : IRatingService
{
    public async Task SaveRatingAsync(string userId, RateMovieDto dto)
    {
        var movie = await context.Movies.FindAsync(dto.MovieId);
        if (movie == null) throw new KeyNotFoundException("Movie not found");

        var existing = await context.Ratings
            .FirstOrDefaultAsync(r => r.UserId == userId && r.MovieId == dto.MovieId);
        
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
            await context.Ratings.AddAsync(rating);
        }

        await context.SaveChangesAsync();
    }

    public async Task<List<object>> GetRatingsAsync(int movieId)
    {
        return await context.Ratings
            .Where(r => r.MovieId == movieId)
            .Include(r => r.User)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new
            {
                r.Id,
                r.Score,
                r.Comment,
                r.CreatedAt,
                UserEmail = r.User.Email
            } as object)
            .ToListAsync();
    }

    public async Task<(double averageScore, int totalRatings)> GetAverageAsync(int movieId)
    {
        var list = await context.Ratings
            .Where(r => r.MovieId == movieId)
            .ToListAsync();
        
        if (!list.Any()) return (0, 0);
        
        var average = list.Average(r => r.Score);
        return (Math.Round(average, 1), list.Count);
    }
}


