using Filmder.Data;
using Filmder.DTOs;
using Filmder.Models;
using Filmder.Extensions;
using Filmder.Signal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Filmder.Controllers;
[ApiController]
public class GameController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IHubContext<ChatHub> _hubContext;
    public GameController(AppDbContext dbContext, IHubContext<ChatHub> hubContext)
    {
        _dbContext = dbContext;
        _hubContext = hubContext;
    }

    [HttpPost("/createAgame")]
    public ActionResult<Game> CreateAGame(CreateGameDto createGameDto)
    {

        var users =  _dbContext.Users.Where(usr => createGameDto.UserEmails.Contains(usr.Email)).ToList();
        var game = new Game
        {
            Name = createGameDto.name,
            Users = users,
            GroupId = createGameDto.groupId,
            Movies = createGameDto.Movies,
            MovieScores = createGameDto.MovieScores
        };

        _dbContext.Games.Add(game);
        _dbContext.SaveChanges();
        return Ok(game);

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
            movieScore = new MovieScore
            {
                MovieId = voteDto.MovieId,
                GameId = voteDto.GameId,
                MovieScoreValue = voteDto.Score
            };
            game.MovieScores.Add(movieScore);
        }
        else
        {
            movieScore.MovieScoreValue += voteDto.Score;
        }

        await _dbContext.SaveChangesAsync();
        
        return Ok();
    }
    
    [HttpGet("/getMoviesBy")]
    public async Task<ActionResult<List<Movie>>> GetMoviesByCriteria(
        [FromQuery] string? genre,
        [FromQuery] int? releaseDate,
        [FromQuery] int? longestDurationMinutes,
        [FromQuery] int movieCount = 10)
    {
        var movies = _dbContext.Movies.AsQueryable();

        if (longestDurationMinutes.HasValue)
            movies = movies.Where(mv => mv.Duration <= longestDurationMinutes.Value);

        if (releaseDate.HasValue)
            movies = movies.Where(mv => mv.ReleaseYear >= releaseDate.Value);

        if (!string.IsNullOrEmpty(genre))
        {
            if (MovieGenreParsingExtensions.TryParseGenre(genre, out var parsedGenre))
                movies = movies.Where(mv => mv.Genre == parsedGenre);
            else
                return BadRequest("Invalid genre");
        }

        var result = await movies.Take(movieCount).ToListAsync();

        return Ok(result);
    }

    
    
     [HttpGet("/getResults/{gameId}")]
    public async Task<ActionResult<List<Movie>>> GetResults(int gameId)
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
            .Take(5) //reikia pakeisti
            .Select(ms => game.Movies.FirstOrDefault(m => m.Id == ms.MovieId))
            .ToList();
        
        topMovies.Sort();

        return Ok(topMovies);
    }
    
    [HttpPost("/endGame/{gameId}")]
    public async Task<ActionResult> EndGame(int gameId)
    {
        var game = await _dbContext.Games.FindAsync(gameId);
        if (game == null)
            return NotFound();

        game.IsActive = false;
        await _dbContext.SaveChangesAsync();

        await _hubContext.Clients.Group(gameId.ToString())
            .SendAsync("gameEnded", $"🎬 Game {game.Name} has ended! View results now.");

        return Ok("Game ended successfully.");
    }
    
    [HttpGet("getActiveGame/{groupId}")]
    public async Task<ActionResult<List<Game>>> GetActiveGames(int groupId)
    {
        var game = await _dbContext.Games
            .Include(g => g.Users)
            .Where(mv => mv.IsActive && mv.GroupId == groupId)
            .ToListAsync();
        
        if (game == null)
        {
            return NotFound();
        }

        return Ok(game);
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
}