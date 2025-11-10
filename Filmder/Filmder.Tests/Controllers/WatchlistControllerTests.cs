using Filmder.Controllers;
using Filmder.DTOs;
using Filmder.Models;
using Filmder.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace Filmder.Tests.Controllers;

public class WatchlistControllerTests
{
    [Fact]
    public async Task GenerateWatchlist_WithRatings_ReturnsMovies()
    {
        var context = TestDbContextFactory.CreateContextWithMovies();
        context.Users.Add(new AppUser { Id = "user1", Email = "test@example.com" });
        
        context.Ratings.AddRange(
            new Rating { UserId = "user1", MovieId = 1, Score = 9 },
            new Rating { UserId = "user1", MovieId = 2, Score = 8 }
        );
        await context.SaveChangesAsync();

        var controller = new WatchlistController(context);
        MockHelpers.SetupControllerContext(controller, "user1", "test@example.com", "test");

        var result = await controller.GenerateWatchlist(count: 10);

        var actionResult = result.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        
        var watchlist = actionResult.Value as List<WatchlistMovieDto>;
        watchlist.Should().NotBeNull();
        watchlist.Should().NotContain(m => m.Id == 1 || m.Id == 2);
    }

    [Fact]
    public async Task GenerateWatchlist_NoPreferences_ReturnsPopularMovies()
    {
        var context = TestDbContextFactory.CreateContextWithMovies();
        context.Users.Add(new AppUser { Id = "user1", Email = "test@example.com" });
        await context.SaveChangesAsync();

        var controller = new WatchlistController(context);
        MockHelpers.SetupControllerContext(controller, "user1", "test@example.com", "test");

        var result = await controller.GenerateWatchlist(count: 10);

        var actionResult = result.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
    }

    [Fact]
    public async Task GetUserPreferences_ReturnsCorrectData()
    {
        var context = TestDbContextFactory.CreateContextWithMovies();
        context.Users.Add(new AppUser { Id = "user1", Email = "test@example.com" });
        context.Ratings.Add(new Rating { UserId = "user1", MovieId = 1, Score = 9 });
        await context.SaveChangesAsync();

        var controller = new WatchlistController(context);
        MockHelpers.SetupControllerContext(controller, "user1", "test@example.com", "test");

        var result = await controller.GetUserPreferences();

        var actionResult = result.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        
        var preferences = actionResult.Value as UserPreferencesDto;
        preferences.Should().NotBeNull();
        preferences.TotalRatings.Should().Be(1);
    }
}