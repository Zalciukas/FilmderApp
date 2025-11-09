using Filmder.Controllers;
using Filmder.Models;
using Filmder.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace Filmder.Tests.Controllers;

public class SwipeControllerTests
{
    [Fact]
    public async Task GetRandomMovie_NoSwipedMovies_ReturnsMovie()
    {
        // Arrange
        var context = TestDbContextFactory.CreateContextWithMovies();
        var controller = new SwipeController(context);
        MockHelpers.SetupControllerContext(controller, "user1", "test@example.com", "test");
        
        // Act
        var result = await controller.GetRandomMovie();

        //Assert
        var actionResult = result.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        
        var movie = actionResult.Value as Movie;
        movie.Should().NotBeNull();
    }

    [Fact]
    public async Task GetRandomMovie_WithGenreFilter_ReturnsFilteredMovie()
    {
        
    }
}