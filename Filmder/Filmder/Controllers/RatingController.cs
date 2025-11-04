using Filmder.DTOs;
using Filmder.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Filmder.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RatingController : ControllerBase
{
    private readonly IRatingService _ratingService;

    public RatingController(IRatingService ratingService)
    {
        _ratingService = ratingService;
    }

    [HttpPost("rate")]
    public async Task<IActionResult> RateMovie([FromBody] RateMovieDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        try
        {
            await _ratingService.SaveRatingAsync(userId, dto);
            return Ok(new { message = "Rating saved successfully" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Movie not found");
        }
    }

    [HttpGet("movie/{movieId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetRatings(int movieId)
    {
        var ratings = await _ratingService.GetRatingsAsync(movieId);
        return Ok(ratings);
    }

    [HttpGet("movie/{movieId}/average")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAverageRating(int movieId)
    {
        var (averageScore, totalRatings) = await _ratingService.GetAverageAsync(movieId);
        return Ok(new { averageScore, totalRatings });
    }
}