using Filmder.DTOs;
using Filmder.Models;
using Filmder.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Filmder.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MovieController : ControllerBase
{
    private readonly IMovieService _movieService;
    private readonly MovieImportService _importService;
    
    public MovieController(MovieImportService importService, IMovieService movieService)
    {
        _movieService = movieService;
        _importService = importService;
    }
    
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<Movie>>> GetAllMovies([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var movies = await _movieService.GetAllAsync(page, pageSize);
        return Ok(movies);
        
        
    }
    
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Movie>> GetMovieById(int id)
    {
        var movie = await _movieService.GetByIdAsync(id);

        if (movie == null)
            return NotFound($"Movie with ID {id} not found");

        return Ok(movie);
    }

    // Paieska pagal pavadinima
    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<ActionResult<List<Movie>>> SearchMovies([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest("Search query cannot be empty");

        var movies = await _movieService.SearchAsync(query);
        return Ok(movies);
    }

    // pagal zanra
    [HttpGet("genre/{genre}")]
    [AllowAnonymous]
    public async Task<ActionResult<List<Movie>>> GetMoviesByGenre(string genre, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var movies = await _movieService.GetByGenreAsync(genre, page, pageSize);
            return Ok(movies);
        }
        catch (ArgumentException)
        {
            return BadRequest("Invalid genre");
        }
        
    }

    // admin funkcija (Speju taip darom)
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Movie>> CreateMovie([FromBody] CreateMovieDto dto)
    {
        try
        {
            var movie = await _movieService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetMovieById), new { id = movie.Id }, movie);
        }
        catch (ArgumentException)
        {
            return BadRequest("Invalid genre");
        }
    }
    
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult> UpdateMovie(int id, [FromBody] CreateMovieDto dto)
    {
        try
        {
            var updated = await _movieService.UpdateAsync(id, dto);
            if (!updated) return NotFound();
            return NoContent();
        }
        catch (ArgumentException)
        {
            return BadRequest("Invalid genre");
        }
    }
    
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> DeleteMovie(int id)
    {
        var deleted = await _movieService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
    
    
    [HttpPost("import")]
    public async Task<IActionResult> ImportMovies()
    {
        int added = await _importService.ImportMoviesFromFileAsync(filePath: "movies.json");

        if (added == 0)
            return BadRequest(new { message = "No movies were imported. The file may be missing, empty, or contain only duplicates." });

        return Ok(new { message = $"{added} movies imported successfully." });
    }



}