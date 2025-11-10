using System.Security.Claims;
using Filmder.Data;
using Filmder.DTOs;
using Filmder.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.CompilerServices;

namespace Filmder.Controllers;

[ApiController]
public class GroupStatsController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public GroupStatsController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [HttpGet("getPlayedGamesCount")]
    public async Task<ActionResult<int>> TotalGamesPlayed(int groupId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return BadRequest();

        var GroupMembers =  await _dbContext.GroupMembers.FirstOrDefaultAsync(gm => gm.UserId == userId && gm.GroupId == groupId);
        if (GroupMembers == null) return Unauthorized();

        var groupGamesCount = await _dbContext.Games.Where(gm => gm.GroupId == groupId).CountAsync();

        return Ok(groupGamesCount);
    }
    
    [HttpGet("getHighestVotedMovie")]
    public async Task<ActionResult<HighestRatedMovieDto>> HighestVotedMovie(int groupId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return BadRequest();

        var GroupMembers =  await _dbContext.GroupMembers.FirstOrDefaultAsync(gm => gm.UserId == userId && gm.GroupId == groupId);
        if (GroupMembers == null) return Unauthorized();

        var movieAndScore = await _dbContext.MovieScores
            .Where(ms => ms.Game.IsActive == false && ms.Game.GroupId == groupId)
            .OrderByDescending(ms => ms.MovieScoreValue)
            .Select(ms => new HighestRatedMovieDto
            {
                Id = ms.Movie.Id,
                Name = ms.Movie.Name,
                Genre = ms.Movie.Genre.ToString(),
                Description = ms.Movie.Description,
                ReleaseYear = ms.Movie.ReleaseYear,
                Rating = ms.Movie.Rating,
                PosterUrl = ms.Movie.PosterUrl ?? string.Empty,
                TrailerUrl = ms.Movie.TrailerUrl ?? string.Empty,
                Duration = ms.Movie.Duration,
                Director = ms.Movie.Director,
                Cast = ms.Movie.Cast,
                CreatedAt = ms.Movie.CreatedAt,
                Score = ms.MovieScoreValue
            })
            .FirstOrDefaultAsync();

        if (movieAndScore == null) return BadRequest();
        
        return Ok(movieAndScore);
    }
    
    [HttpGet("getHighestVotedGenre")]
    public async Task<ActionResult<HighestRatedMovieDto>> HighestVotedGenre(int groupId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return BadRequest();

        var GroupMembers =  await _dbContext.GroupMembers.FirstOrDefaultAsync(gm => gm.UserId == userId && gm.GroupId == groupId);
        if (GroupMembers == null) return Unauthorized();

        var mostPopular = await _dbContext.MovieScores
            .Where(ms => !ms.Game.IsActive && ms.Game.GroupId == groupId)
            .GroupBy(ms => ms.Movie.Genre)
            .Select(g => new PopularGenreDto
            {
                Genre = g.Key.ToString(),
                TotalScore = g.Sum(ms => ms.MovieScoreValue)
            })
            .OrderByDescending(g => g.TotalScore)
            .FirstOrDefaultAsync();

        return mostPopular == null ? NotFound() : Ok(mostPopular);
        
    }
    
    [HttpGet("averageMovieScore")]
    public async Task<ActionResult<double>> GetAverageMovieScore(int groupId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return BadRequest();

        var query = _dbContext.MovieScores
            .Where(ms => !ms.Game.IsActive && ms.Game.GroupId == groupId);

        if (!await query.AnyAsync())
            return NotFound();

        double averageScore = await query.AverageAsync(ms => ms.MovieScoreValue);

        return Ok(Math.Round(averageScore, 2)); 
    }
    
    [HttpGet("averageMovieDuration")]
    public async Task<ActionResult<double>> GetAverageMovieDuration(int groupId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return BadRequest("User not found.");

        var query = _dbContext.MovieScores
            .Where(ms => !ms.Game.IsActive && ms.Game.GroupId == groupId);

        if (!await query.AnyAsync())
            return NotFound("No finished games found for this group.");

        double averageDuration = await query.AverageAsync(ms => ms.Movie.Duration);

        return Ok(Math.Round(averageDuration, 2));
    }
}