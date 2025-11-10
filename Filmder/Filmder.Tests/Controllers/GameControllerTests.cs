using Filmder.Controllers;
using Filmder.DTOs;
using Filmder.Models;
using Filmder.Signal;
using Filmder.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Xunit;

namespace Filmder.Tests.Controllers;

public class GameControllerTests
{
    private readonly Mock<IHubContext<ChatHub>> _mockHubContext;

    public GameControllerTests()
    {
        _mockHubContext = new Mock<IHubContext<ChatHub>>();
    }

    [Fact]
    public void CreateAGame_ValidDto_CreatesGame()
    {
        // Arrange
        var context = TestDbContextFactory.CreateContextWithMovies();
        context.Users.AddRange(
            new AppUser { Id = "user1", Email = "test1@example.com", UserName = "test1" },
            new AppUser { Id = "user2", Email = "test2@example.com", UserName = "test2" }
        );
        context.SaveChanges();

        var controller = new GameController(context, _mockHubContext.Object);

        var createDto = new CreateGameDto
        {
            name = "Friday Movie Night",
            UserEmails = new List<string> { "test1@example.com", "test2@example.com" },
            groupId = 1,
            Movies = new List<Movie>(),
            MovieScores = new List<MovieScore>()
        };

        // Act
        var result = controller.CreateAGame(createDto);

        // Assert
        var actionResult = result.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        
        var game = actionResult.Value as Game;
        game.Should().NotBeNull();
        game.Name.Should().Be("Friday Movie Night");
        game.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Vote_ValidVote_UpdatesMovieScore()
    {
        // Arrange
        var context = TestDbContextFactory.CreateContextWithMovies();
        var game = new Game
        {
            Name = "Test Game",
            GroupId = 1,
            IsActive = true,
            MovieScores = new List<MovieScore>()
        };
        context.Games.Add(game);
        await context.SaveChangesAsync();

        var controller = new GameController(context, _mockHubContext.Object);

        var voteDto = new VoteDto
        {
            GameId = game.Id,
            MovieId = 1,
            Score = 10
        };

        // Act
        var result = await controller.Vote(voteDto);

        // Assert
        result.Should().BeOfType<OkResult>();
        
        var updatedGame = context.Games.First(g => g.Id == game.Id);
        updatedGame.MovieScores.Should().HaveCount(1);
        updatedGame.MovieScores.First().MovieScoreValue.Should().Be(10);
    }

    [Fact]
    public async Task Vote_ExistingMovieScore_AddsToScore()
    {
        // Arrange
        var context = TestDbContextFactory.CreateContextWithMovies();
        var game = new Game
        {
            Name = "Test Game",
            GroupId = 1,
            IsActive = true,
            MovieScores = new List<MovieScore>
            {
                new MovieScore { MovieId = 1, GameId = 1, MovieScoreValue = 5 }
            }
        };
        context.Games.Add(game);
        await context.SaveChangesAsync();

        var controller = new GameController(context, _mockHubContext.Object);

        var voteDto = new VoteDto
        {
            GameId = game.Id,
            MovieId = 1,
            Score = 10
        };

        // Act
        var result = await controller.Vote(voteDto);

        // Assert
        result.Should().BeOfType<OkResult>();
        
        var movieScore = context.MovieScores.First(ms => ms.MovieId == 1 && ms.GameId == game.Id);
        movieScore.MovieScoreValue.Should().Be(15); // 5 + 10
    }

    [Fact]
    public async Task Vote_NonExistentGame_ReturnsBadRequest()
    {
        // Arrange
        var context = TestDbContextFactory.CreateContextWithMovies();
        var controller = new GameController(context, _mockHubContext.Object);

        var voteDto = new VoteDto
        {
            GameId = 999,
            MovieId = 1,
            Score = 10
        };

        // Act
        var result = await controller.Vote(voteDto);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task GetMoviesByCriteria_InvalidGenre_ReturnsBadRequest()
    {
        // Arrange
        var context = TestDbContextFactory.CreateContextWithMovies();
        var controller = new GameController(context, _mockHubContext.Object);

        // Act
        var result = await controller.GetMoviesByCriteria(genre: "InvalidGenre", releaseDate: 1999, longestDurationMinutes: 200);

        // Assert
        var actionResult = result.Result as BadRequestObjectResult;
        actionResult.Should().NotBeNull();
    }

    [Fact]
    public async Task EndGame_ValidGame_SetsIsActiveToFalse()
    {
        // Arrange
        var context = TestDbContextFactory.CreateContextWithMovies();
        var game = new Game
        {
            Name = "Test Game",
            GroupId = 1,
            IsActive = true
        };
        context.Games.Add(game);
        await context.SaveChangesAsync();

        var mockClients = new Mock<IHubClients>();
        var mockClientProxy = new Mock<IClientProxy>();
        mockClients.Setup(c => c.Group(It.IsAny<string>())).Returns(mockClientProxy.Object);
        _mockHubContext.Setup(h => h.Clients).Returns(mockClients.Object);

        var controller = new GameController(context, _mockHubContext.Object);

        // Act
        var result = await controller.EndGame(game.Id);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        
        var updatedGame = await context.Games.FindAsync(game.Id);
        updatedGame.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task EndGame_NonExistentGame_ReturnsNotFound()
    {
        // Arrange
        var context = TestDbContextFactory.CreateContextWithMovies();
        var controller = new GameController(context, _mockHubContext.Object);

        // Act
        var result = await controller.EndGame(999);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetActiveGames_ReturnsOnlyActiveGames()
    {
        // Arrange
        var context = TestDbContextFactory.CreateContextWithMovies();
        context.Games.AddRange(
            new Game { Name = "Active Game", GroupId = 1, IsActive = true },
            new Game { Name = "Inactive Game", GroupId = 1, IsActive = false },
            new Game { Name = "Other Group Game", GroupId = 2, IsActive = true }
        );
        await context.SaveChangesAsync();

        var controller = new GameController(context, _mockHubContext.Object);

        // Act
        var result = await controller.GetActiveGames(1);

        // Assert
        var actionResult = result.Result as OkObjectResult;
        var games = actionResult.Value as List<Game>;
        games.Should().HaveCount(1);
        games[0].Name.Should().Be("Active Game");
    }

    [Fact]
    public async Task GetResults_ValidGame_ReturnsTopMovies()
    {
        // Arrange
        var context = TestDbContextFactory.CreateContextWithMovies();
        var game = new Game
        {
            Name = "Test Game",
            GroupId = 1,
            IsActive = false,
            Movies = context.Movies.ToList(),
            MovieScores = new List<MovieScore>
            {
                new MovieScore { MovieId = 1, GameId = 1, MovieScoreValue = 100 },
                new MovieScore { MovieId = 2, GameId = 1, MovieScoreValue = 90 }
            }
        };
        context.Games.Add(game);
        await context.SaveChangesAsync();

        var controller = new GameController(context, _mockHubContext.Object);

        // Act
        var result = await controller.GetResults(game.Id);

        // Assert
        var actionResult = result.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
    }
}