using Filmder.Controllers;
using Filmder.DTOs;
using Filmder.Models;
using Filmder.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace Filmder.Tests.Controllers;

public class GroupStatsControllerTests
{
    [Fact]
    public async Task TotalGamesPlayed_ValidUser_ReturnsCount()
    {
        // Arrange
        var context = TestDbContextFactory.CreateContextWithMovies();
        
        context.Groups.Add(new Group { Id = 1, Name = "Test Group", OwnerId = "user1" });
        context.GroupMembers.Add(new GroupMember { UserId = "user1", GroupId = 1 });
        context.Games.AddRange(
            new Game { Name = "Game 1", GroupId = 1, IsActive = false },
            new Game { Name = "Game 2", GroupId = 1, IsActive = false }
        );
        await context.SaveChangesAsync();

        var controller = new GroupStatsController(context);
        MockHelpers.SetupControllerContext(controller, "user1", "test@example.com", "test");

        // Act
        var result = await controller.TotalGamesPlayed(1);

        // Assert
        var actionResult = result.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        
        var count = (int)actionResult.Value;
        count.Should().Be(2); // Changed from 3 to 2 to match actual data
    }
    
    [Fact]
    public async Task TotalGamesPlayed_UserNotInGroup_ReturnsUnauthorized()
    {
        // Arrange
        var context = TestDbContextFactory.CreateContextWithMovies();
        context.Groups.Add(new Group { Id = 1, Name = "Test Group", OwnerId = "user2" });
        await context.SaveChangesAsync();

        var controller = new GroupStatsController(context);
        MockHelpers.SetupControllerContext(controller, "user1", "test@example.com", "test");

        // Act
        var result = await controller.TotalGamesPlayed(1);

        // Assert
        result.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task HighestVotedMovie_ValidGroup_ReturnsHighestScoredMovie()
    {
        // Arrange
        var context = TestDbContextFactory.CreateContextWithMovies();
        
        context.Groups.Add(new Group { Id = 1, Name = "Test Group", OwnerId = "user1" });
        context.GroupMembers.Add(new GroupMember { UserId = "user1", GroupId = 1 });
        
        var game = new Game 
            { 
                Name = "Test Game",
                GroupId = 1,
                IsActive = false,
                MovieScores = new List<MovieScore>
                {
                new MovieScore { MovieId = 1, GameId = 1, MovieScoreValue = 100 },
                new MovieScore { MovieId = 2, GameId = 1, MovieScoreValue = 150 }
                }
            };
        
        context.Games.Add(game);
        await context.SaveChangesAsync();
        
        var controller = new GroupStatsController(context);
        MockHelpers.SetupControllerContext(controller, "user1", "test1@example.com", "test1");
        
        // Act
        var result = await controller.HighestVotedMovie(1);
        
        // Assert
        var actionResult = result.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        
        var movieDto = actionResult.Value as HighestRatedMovieDto;
        movieDto.Should().NotBeNull();
        movieDto.Score.Should().Be(150);
        movieDto.Name.Should().Be("Inception");
    }
    
    [Fact]
    public async Task HighestVotedGenre_ValidGroup_ReturnsMostPopularGenre()
    {
        // Arrange
        var context = TestDbContextFactory.CreateContextWithMovies();
        
        context.Groups.Add(new Group { Id = 1, Name = "Test Group", OwnerId = "user1" });
        context.GroupMembers.Add(new GroupMember { UserId = "user1", GroupId = 1 });
        
        var game = new Game
        {
            Name = "Test Game",
            GroupId = 1,
            IsActive = false,
            MovieScores = new List<MovieScore>
            {
                new MovieScore { MovieId = 1, GameId = 1, MovieScoreValue = 100 },
                new MovieScore { MovieId = 2, GameId = 1, MovieScoreValue = 150 },
                new MovieScore { MovieId = 3, GameId = 1, MovieScoreValue = 50 }
            }
        };
        context.Games.Add(game);
        await context.SaveChangesAsync();

        var controller = new GroupStatsController(context);
        MockHelpers.SetupControllerContext(controller, "user1", "test@example.com", "test");

        // Act
        var result = await controller.HighestVotedGenre(1);

        // Assert
        var actionResult = result.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        
        var genreDto = actionResult.Value as PopularGenreDto;
        genreDto.Should().NotBeNull();
        genreDto.Genre.Should().Be("SciFi");
        genreDto.TotalScore.Should().Be(250);
    }

    [Fact]
    public async Task GetAverageMovieScore_ValidGroup_ReturnsAverage()
    {
        //Arannge
        var context = TestDbContextFactory.CreateContextWithMovies();
        context.Groups.Add(new Group { Id = 1, Name = "Test Group", OwnerId = "user1" });

        var game = new Game
        {
            Name = "Test Game",
            GroupId = 1,
            IsActive = false,
            MovieScores = new List<MovieScore>
            {
                new MovieScore { MovieId = 1, GameId = 1, MovieScoreValue = 0 },
                new MovieScore { MovieId = 2, GameId = 1, MovieScoreValue = 100 },
                new MovieScore { MovieId = 3, GameId = 1, MovieScoreValue = 50 } 
            }
        };
        context.Games.Add(game);
        await context.SaveChangesAsync();
        
        var controller = new GroupStatsController(context);
        MockHelpers.SetupControllerContext(controller, "user1", "test@example.com", "test");
        
        //Act
        var result = await controller.GetAverageMovieScore(1);
        
        //Assert
        var actionResult = result.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        
        var average = (double)actionResult.Value;
        average.Should().Be(50.0);
    }
    
    [Fact]
    public async Task GetAverageMovieDuration_ValidGroup_ReturnsAverageDuration()
    {
        // Arrange
        var context = TestDbContextFactory.CreateContextWithMovies();
        
        context.Groups.Add(new Group { Id = 1, Name = "Test Group", OwnerId = "user1" });
        
        var game = new Game
        {
            Name = "Test Game",
            GroupId = 1,
            IsActive = false,
            MovieScores = new List<MovieScore>
            {
                new MovieScore { MovieId = 1, GameId = 1, MovieScoreValue = 100 }, // Duration: 136
                new MovieScore { MovieId = 2, GameId = 1, MovieScoreValue = 100 },  // Duration: 148
                new MovieScore { MovieId = 3,  GameId = 1, MovieScoreValue = 100 } // Duration : 175
            }
        };
        context.Games.Add(game);
        await context.SaveChangesAsync();

        var controller = new GroupStatsController(context);
        MockHelpers.SetupControllerContext(controller, "user1", "test@example.com", "test");

        // Act
        var result = await controller.GetAverageMovieDuration(1);

        // Assert
        var actionResult = result.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        
        var average = (double)actionResult.Value;
        average.Should().Be(153.0); // (136+148+175)/3=153
    }
}