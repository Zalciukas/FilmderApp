using Filmder.Data;
using Filmder.DTOs;
using Filmder.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Filmder.Controllers;
[ApiController]
public class GameController : ControllerBase
{
    private AppDbContext _dbContext;

    public GameController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost("/createAgame")]
    public ActionResult<Game> CreateAGame(CreateGameDto createGameDto)
    {
        var game = new Game
        {
            Name = createGameDto.name,
            Users = createGameDto.Users,
            Movies = createGameDto.Movies,
            MovieScores = createGameDto.MovieScores
        };

        return game;

    }
    
    [HttpPost("/vote")]
    public async Task<ActionResult> Vote(VoteDto voteDto)
    {

        var game = await _dbContext.Games.Include(g => g.MovieScores).FirstOrDefaultAsync(g => g.Id == voteDto.GameId);
        if (game == null)
        {
            return BadRequest();
            
        }

        var movieScore = game.MovieScores.FirstOrDefault(ms => ms.MovieId == voteDto.MovieId);
        if (movieScore == null)
        {

            var newMovieScore = new MovieScore
            {
                MovieId = voteDto.MovieId,
                GameId = voteDto.GameId,
                MovieScoreValue = voteDto.Score
            };
            game.MovieScores.Add(newMovieScore);
            await _dbContext.SaveChangesAsync();
        }

        movieScore.MovieScoreValue = +voteDto.Score;
        
        
        return Ok();

    }
    
    
    
    [HttpGet("/getMoviesBy")]
    public async Task<ActionResult<List<Movie>>> getMoviesByCriteria(GetMoviesByCriteriaDto getMoviesByCriteriaDto)
        {
            var movies = _dbContext.Movies.AsQueryable();
            
            
            if(getMoviesByCriteriaDto.LongestDurationMinutes !=null){ movies.Where(mv => mv.Duration <= getMoviesByCriteriaDto.LongestDurationMinutes); }
            if (getMoviesByCriteriaDto.ReleaseDate != null){ movies.Where(mv =>mv.ReleaseYear >= getMoviesByCriteriaDto.ReleaseDate); }
            if (getMoviesByCriteriaDto.LongestDurationMinutes != null){ movies.Where(mv => mv.Genre == getMoviesByCriteriaDto.Genre); }
            
           var result = await movies.Take(getMoviesByCriteriaDto.MovieCount).ToListAsync();
            
            return result;
        }
    
    
     [HttpGet("/getResults{gameId}")]
    public async Task<ActionResult<List<Movie>>> getresults(int gameId)
    {
        var game = await _dbContext.Games
            .Include(gm => gm.MovieScores)
            .Include(gm => gm.Movies)
            .FirstOrDefaultAsync(gm => gm.Id == gameId);
        
        
        if (game == null)
        {
            return BadRequest();
        }
        

        var topMovies = game.MovieScores
            .OrderByDescending(ms => ms.MovieScoreValue)
            .Take(5) //reikia pakeisti
            .Select(ms => game.Movies.FirstOrDefault(m => m.Id == ms.MovieId))
            .ToList();

        return topMovies;
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
}