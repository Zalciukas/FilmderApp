using Filmder.Data;
using Filmder.Models;
using Microsoft.EntityFrameworkCore;

namespace Filmder.Repositories;

public class RatingRepository(AppDbContext context) : IRatingRepository
{
    public Task<Movie?> FindMovieAsync(int movieId)
    {
        return context.Movies.FindAsync(movieId).AsTask();
    }

    public Task<Rating?> FindUserRatingAsync(string userId, int movieId)
    {
        return context.Ratings.FirstOrDefaultAsync(r => r.UserId == userId && r.MovieId == movieId);
    }

    public async Task AddRatingAsync(Rating rating)
    {
        await context.Ratings.AddAsync(rating);
    }

    public Task SaveChangesAsync()
    {
        return context.SaveChangesAsync();
    }

    public Task<List<object>> GetMovieRatingsProjectionAsync(int movieId)
    {
        return context.Ratings
            .Where(r => r.MovieId == movieId)
            .Include(r => r.User)
            .Select(r => new
            {
                r.Id,
                r.Score,
                r.Comment,
                r.CreatedAt,
                UserEmail = r.User.Email
            } as object)
            .OrderByDescending(r => EF.Property<DateTime>(r, "CreatedAt"))
            .ToListAsync();
    }

    public Task<List<Rating>> GetMovieRatingsAsync(int movieId)
    {
        return context.Ratings
            .Where(r => r.MovieId == movieId)
            .ToListAsync();
    }
}


