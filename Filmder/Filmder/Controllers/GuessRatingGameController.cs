using System.Security.Claims;
using Filmder.Data;
using Filmder.DTOs;
using Filmder.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Filmder.Controllers;


[ApiController]
public class GuessRatingGameController : ControllerBase
{

    private readonly AppDbContext _dbContext;

    public GuessRatingGameController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost("groups/{groupId}/guessing-games")]
    public async Task<ActionResult<GuessRatingGame>> CreateGame(int groupId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return BadRequest();
       

        var movieCount = await _dbContext.Movies.CountAsync();
        
        var random = new Random(Guid.NewGuid().GetHashCode());
        int index = random.Next(movieCount);        

        var movieIds = await _dbContext.Movies
            .OrderBy(m => m.Id)
            .Skip(Math.Max(0, index - 10))
            .Take(10)
            .Select(m => m.Id)
            .ToListAsync();

        if (!movieIds.Any())
            return NotFound();

        var movies = await _dbContext.Movies
            .Where(m => movieIds.Contains(m.Id))
            .ToListAsync();

        var guessRatingGame = new GuessRatingGame
        {
            GroupId = groupId,
            UserId = userId,
            Movies = movies
        };

        _dbContext.RatingGuessingGames.Add(guessRatingGame);
        await _dbContext.SaveChangesAsync();
        
        return Ok(guessRatingGame);
        
    }
    
    
    [HttpGet("games/{gameId}/guessing-games")]
    public async Task<ActionResult<GuessRatingGame>> GetGameMovies(int gameId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return BadRequest();


        var game = await  _dbContext.RatingGuessingGames
            .Include(g => g.Movies)
            .Include(g => g.Group)
            .ThenInclude(g => g.GroupMembers)
            .FirstOrDefaultAsync(g => g.Id == gameId);
        if (game == null) return NotFound();

        var userPartOfGroup = game.Group.GroupMembers.Any(gm => gm.UserId == userId);
        if (!userPartOfGroup) return Forbid();

        var movies = game.Movies;
        if (!movies.Any()) return NotFound();
        
        
        return Ok(movies);
    }
    
    [HttpPost("games/{gameId}/movies/{movieId}/guesses")]
    public async Task<ActionResult<GuessRatingGame>> GuessRating(int gameId,int movieId, RatingGuessDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return BadRequest();


        var game = await  _dbContext.RatingGuessingGames
            .Include(g => g.Movies)
            .Include(g => g.Group)
            .ThenInclude(g => g.GroupMembers)
            .Include(g => g.Guesses)
            .FirstOrDefaultAsync(g => g.Id == gameId);
        if (game == null) return NotFound();

        var userPartOfGroup = game.Group.GroupMembers.Any(gm => gm.UserId == userId);
        if (!userPartOfGroup) return Forbid();
        
        if (!game.Movies.Any(m => m.Id == movieId))
            return BadRequest();


        var guess = new MovieRatingGuess
        {
            MovieId = movieId,
            UserId = userId,
            RatingGuessValue = dto.RatingGuessValue
        };
        game.Guesses.Add(guess);
        await _dbContext.SaveChangesAsync();
        
        return Ok(guess);
    }
    
    
    [HttpPost("games/{gameId}/finish")]
    public async Task<ActionResult<GuessRatingGame>> FinishGame(int gameId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return BadRequest();


        var game = await  _dbContext.RatingGuessingGames
            .Include(g => g.Group)
            .ThenInclude(g => g.GroupMembers)
            .FirstOrDefaultAsync(g => g.Id == gameId);
        if (game == null) return NotFound();

        var userPartOfGroup = game.Group.GroupMembers.Any(gm => gm.UserId == userId);
        if (!userPartOfGroup) return Forbid();


        game.IsActive = false;
        await _dbContext.SaveChangesAsync();
        
        return Ok(game);
    }
    
    
    [HttpGet("games/{gameId}/results")]
    public async Task<ActionResult> GetGameResults(int gameId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return BadRequest();

        var game = await _dbContext.RatingGuessingGames
            .Include(g => g.Group)
            .ThenInclude(g => g.GroupMembers)
            .Include(g => g.Movies)
            .Include(g => g.Guesses)
            .ThenInclude(g => g.User) // optional, if you want usernames
            .FirstOrDefaultAsync(g => g.Id == gameId);

        if (game == null) return NotFound();

        var userPartOfGroup = game.Group.GroupMembers.Any(gm => gm.UserId == userId);
        if (!userPartOfGroup) return Forbid();

        var results = game.Guesses
            .GroupBy(g => g.UserId)
            .Select(group => new
            {
                UserId = group.Key,
                Username = group.FirstOrDefault()?.User?.UserName, // optional if user navigation exists
                TotalDifference = group.Sum(g =>
                {
                    var movie = game.Movies.FirstOrDefault(m => m.Id == g.MovieId);
                    return movie == null ? 0 : Math.Abs(movie.Rating - g.RatingGuessValue);
                }),
                AverageDifference = group.Average(g =>
                {
                    var movie = game.Movies.FirstOrDefault(m => m.Id == g.MovieId);
                    return movie == null ? 0 : Math.Abs(movie.Rating - g.RatingGuessValue);
                })
            })
            .OrderBy(x => x.TotalDifference)
            .ToList();

        return Ok(results);
    }


}