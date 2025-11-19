using Filmder.Controllers;
using Filmder.DTOs;
using Filmder.Models;
using Filmder.Services;
using Filmder.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Filmder.Tests.Controllers;

public class MovieControllerTests
{
    private readonly Mock<IMovieCacheService> _mockCacheService;

    public MovieControllerTests()
    {
        _mockCacheService = new Mock<IMovieCacheService>();
    }

    [Fact]
    public async Task GetAllMovies_ReturnsOkWithMovies()
    {
        // Arrange
        var context = TestDbContextFactory.CreateContextWithMovies();
        var mockImportService = new Mock<MovieImportService>(context);
        var controller = new MovieController(context, mockImportService.Object, _mockCacheService.Object);

        // Act
        var result = await controller.GetAllMovies(page: 1, pageSize: 20);

        // Assert
        var actionResult = result.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        
        var movies = actionResult.Value as List<Movie>;
        movies.Should().NotBeNull();
        movies.Should().HaveCount(4);
    }

    [Fact]
    public async Task GetAllMovies_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var context = TestDbContextFactory.CreateContextWithMovies();
        var mockImportService = new Mock<MovieImportService>(context);
        var controller = new MovieController(context, mockImportService.Object, _mockCacheService.Object);

        // Act
        var result = await controller.GetAllMovies(page: 1, pageSize: 2);

        // Assert
        var actionResult = result.Result as OkObjectResult;
        var movies = actionResult.Value as List<Movie>;
        movies.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetMovieById_ExistingId_ReturnsMovie()
    {
        // Arrange
        var context = TestDbContextFactory.CreateContextWithMovies();
        var mockImportService = new Mock<MovieImportService>(context);
        
        var testMovie = new Movie
        {
            Id = 1,
            Name = "The Matrix",
            Genre = MovieGenre.SciFi
        };
        
        _mockCacheService.Setup(x => x.GetMovieByIdAsync(1))
            .ReturnsAsync(testMovie);
        
        var controller = new MovieController(context, mockImportService.Object, _mockCacheService.Object);

        // Act
        var result = await controller.GetMovieById(1);

        // Assert
        var actionResult = result.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        
        var movie = actionResult.Value as Movie;
        movie.Should().NotBeNull();
        movie.Id.Should().Be(1);
        movie.Name.Should().Be("The Matrix");
    }

    [Fact]
    public async Task GetMovieById_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var context = TestDbContextFactory.CreateContextWithMovies();
        var mockImportService = new Mock<MovieImportService>(context);
        
        _mockCacheService.Setup(x => x.GetMovieByIdAsync(999))
            .ReturnsAsync((Movie?)null);
        
        var controller = new MovieController(context, mockImportService.Object, _mockCacheService.Object);

        // Act
        var result = await controller.GetMovieById(999);

        // Assert
        var actionResult = result.Result as NotFoundObjectResult;
        actionResult.Should().NotBeNull();
    }

    [Fact]
    public async Task SearchMovies_WithQuery_ReturnsMatchingMovies()
    {
        // Arrange
        var context = TestDbContextFactory.CreateContextWithMovies();
        var mockImportService = new Mock<MovieImportService>(context);
        var controller = new MovieController(context, mockImportService.Object, _mockCacheService.Object);

        // Act
        var result = await controller.SearchMovies("Matrix");

        // Assert
        var actionResult = result.Result as OkObjectResult;
        var movies = actionResult.Value as List<Movie>;
        movies.Should().HaveCount(1);
        movies[0].Name.Should().Contain("Matrix");
    }

    [Fact]
    public async Task SearchMovies_EmptyQuery_ReturnsBadRequest()
    {
        // Arrange
        var context = TestDbContextFactory.CreateContextWithMovies();
        var mockImportService = new Mock<MovieImportService>(context);
        var controller = new MovieController(context, mockImportService.Object, _mockCacheService.Object);

        // Act
        var result = await controller.SearchMovies("");

        // Assert
        var actionResult = result.Result as BadRequestObjectResult;
        actionResult.Should().NotBeNull();
    }

    [Fact]
    public async Task GetMoviesByGenre_ValidGenre_ReturnsFilteredMovies()
    {
        // Arrange
        var context = TestDbContextFactory.CreateContextWithMovies();
        var mockImportService = new Mock<MovieImportService>(context);
        var controller = new MovieController(context, mockImportService.Object, _mockCacheService.Object);

        // Act
        var result = await controller.GetMoviesByGenre("SciFi", page: 1, pageSize: 20);

        // Assert
        var actionResult = result.Result as OkObjectResult;
        var movies = actionResult.Value as List<Movie>;
        movies.Should().HaveCount(2);
        movies.Should().OnlyContain(m => m.Genre == MovieGenre.SciFi);
    }

    [Fact]
    public async Task GetMoviesByGenre_InvalidGenre_ReturnsBadRequest()
    {
        // Arrange
        var context = TestDbContextFactory.CreateContextWithMovies();
        var mockImportService = new Mock<MovieImportService>(context);
        var controller = new MovieController(context, mockImportService.Object, _mockCacheService.Object);

        // Act
        var result = await controller.GetMoviesByGenre("InvalidGenre");

        // Assert
        var actionResult = result.Result as BadRequestObjectResult;
        actionResult.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateMovie_ValidDto_CreatesMovie()
    {
        // Arrange
        var context = TestDbContextFactory.CreateInMemoryContext();
        var mockImportService = new Mock<MovieImportService>(context);
        var controller = new MovieController(context, mockImportService.Object, _mockCacheService.Object);
        MockHelpers.SetupControllerContext(controller, "user1", "test@example.com", "test");

        var dto = new CreateMovieDto
        {
            Name = "New Movie",
            Genre = "Action",
            Description = "A new action movie",
            ReleaseYear = 2023,
            Rating = 8.0,
            Duration = 120,
            Director = "Director Name",
            Cast = "Actor 1, Actor 2"
        };

        // Act
        var result = await controller.CreateMovie(dto);

        // Assert
        var actionResult = result.Result as CreatedAtActionResult;
        actionResult.Should().NotBeNull();
        
        var movie = actionResult.Value as Movie;
        movie.Should().NotBeNull();
        movie.Name.Should().Be(dto.Name);
    }

    [Fact]
    public async Task UpdateMovie_ExistingMovie_UpdatesSuccessfully()
    {
        // Arrange
        var context = TestDbContextFactory.CreateContextWithMovies();
        var mockImportService = new Mock<MovieImportService>(context);
        var controller = new MovieController(context, mockImportService.Object, _mockCacheService.Object);
        MockHelpers.SetupControllerContext(controller, "user1", "test@example.com", "test");

        var dto = new CreateMovieDto
        {
            Name = "Updated Name",
            Genre = "Drama",
            Description = "Updated description",
            ReleaseYear = 2023,
            Rating = 9.0,
            Duration = 150,
            Director = "New Director",
            Cast = "New Cast"
        };

        // Act
        var result = await controller.UpdateMovie(1, dto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        
        var updatedMovie = await context.Movies.FindAsync(1);
        updatedMovie.Name.Should().Be("Updated Name");
        updatedMovie.Genre.Should().Be(MovieGenre.Drama);
        
        // Verify cache was invalidated
        _mockCacheService.Verify(x => x.InvalidateCache(1), Times.Once);
    }

    [Fact]
    public async Task DeleteMovie_ExistingMovie_DeletesSuccessfully()
    {
        // Arrange
        var context = TestDbContextFactory.CreateContextWithMovies();
        var mockImportService = new Mock<MovieImportService>(context);
        var controller = new MovieController(context, mockImportService.Object, _mockCacheService.Object);
        MockHelpers.SetupControllerContext(controller, "user1", "test@example.com", "test");

        // Act
        var result = await controller.DeleteMovie(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        
        var deletedMovie = await context.Movies.FindAsync(1);
        deletedMovie.Should().BeNull();
        
        // Verify cache was invalidated
        _mockCacheService.Verify(x => x.InvalidateCache(1), Times.Once);
    }

    [Fact]
    public async Task GetDailyMovie_ReturnsMovieWithCountdown()
    {
        // Arrange
        var context = TestDbContextFactory.CreateContextWithMovies();
        var mockImportService = new Mock<MovieImportService>(context);
        var controller = new MovieController(context, mockImportService.Object, _mockCacheService.Object);

        // Act
        var result = await controller.GetDailyMovie();

        // Assert
        var actionResult = result.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        
        var dailyMovie = actionResult.Value as DailyMovieDto;
        dailyMovie.Should().NotBeNull();
        dailyMovie.Name.Should().NotBeNullOrEmpty();
        dailyMovie.CountdownSeconds.Should().BeGreaterThan(0);
    }
}