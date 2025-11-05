using Filmder.DTOs;
using Filmder.Models;
using Filmder.Signal;
using Filmder.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Filmder.Controllers;
[ApiController]
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;
    private readonly IHubContext<ChatHub> _hubContext;
    public GameController(IHubContext<ChatHub> hubContext, IGameService gameService)
    {
        _hubContext = hubContext;
        _gameService = gameService;
    }

    [HttpPost("/createAgame")]
    public async Task<ActionResult<Game>> CreateAGame(CreateGameDto createGameDto)
    {
        var game = await _gameService.CreateAsync(createGameDto);
        return Ok(game);

    }
    
    [HttpPost("/vote")]
    public async Task<ActionResult> Vote(VoteDto voteDto)
    {

        try
        {
            await _gameService.VoteAsync(voteDto);
            return Ok();
        }
        catch
        {
            return BadRequest();
        }

    }
    
    [HttpGet("/getMoviesBy")]
    public async Task<ActionResult<List<Movie>>> GetMoviesByCriteria(
        [FromQuery] string? genre,
        [FromQuery] int? releaseDate,
        [FromQuery] int? longestDurationMinutes,
        [FromQuery] int movieCount = 10)
    {
        try
        {
            var result = await _gameService.GetMoviesByCriteriaAsync(genre, releaseDate, longestDurationMinutes, movieCount);
            return Ok(result);
        }
        catch (ArgumentException)
        {
            return BadRequest("Invalid genre");
        }
    }

    
    
     [HttpGet("/getResults{gameId}")]
    public async Task<ActionResult<List<Movie>>> getresults(int gameId)
    {
        try
        {
            var topMovies = await _gameService.GetResultsAsync(gameId);
            return topMovies;
        }
        catch
        {
            return BadRequest();
        }
    }
    
    [HttpPost("/endGame/{gameId}")]
    public async Task<ActionResult> EndGame(int gameId)
    {
        var ended = await _gameService.EndGameAsync(gameId);
        if (!ended) return NotFound();
        await _hubContext.Clients.Group(gameId.ToString())
            .SendAsync("gameEnded", $"🎬 Game {gameId} has ended! View results now.");
        return Ok("Game ended successfully.");
    }
    
    [HttpGet("getActiveGame/{groupId}")]
    public async Task<ActionResult<List<Game>>> GetActiveGames(int groupId)
    {
        var game = await _gameService.GetActiveGamesAsync(groupId);
        if (game == null) { return NotFound(); }
        return Ok(game);
        
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
}