using Filmder.Models;
using FluentAssertions;
using Xunit;

namespace Filmder.Tests.Models;

public class MovieTests
{
    [Fact]
    public void MovieDuration_ConvertsMinutesToHoursAndMinutes()
    {
        // Arrange & Act
        var duration = new MovieDuration(150);

        // Assert
        duration.Hours.Should().Be(2);
        duration.Minutes.Should().Be(30);
        duration.TotalMinutes.Should().Be(150);
        duration.ToString().Should().Be("2h 30m");
    }

    [Fact]
    public void MovieDuration_LessThanOneHour_ShowsZeroHours()
    {
        // Arrange & Act
        var duration = new MovieDuration(45);

        // Assert
        duration.Hours.Should().Be(0);
        duration.Minutes.Should().Be(45);
        duration.ToString().Should().Be("0h 45m");
    }

    [Fact]
    public void Movie_GetFormattedDuration_ReturnsMovieDuration()
    {
        // Arrange
        var movie = new Movie { Duration = 120 };

        // Act
        var formatted = movie.GetFormattedDuration();

        // Assert
        formatted.Hours.Should().Be(2);
        formatted.Minutes.Should().Be(0);
    }

    [Fact]
    public void Movie_CompareTo_SortsByRating_ThenReleaseYear()
    {
        // Arrange
        var movie1 = new Movie { Name = "Movie1", Rating = 8.5, ReleaseYear = 2020 };
        var movie2 = new Movie { Name = "Movie2", Rating = 9.0, ReleaseYear = 2019 };
        var movie3 = new Movie { Name = "Movie3", Rating = 8.5, ReleaseYear = 2021 };

        var movies = new List<Movie> { movie1, movie2, movie3 };

        // Act
        movies.Sort();

        // Assert
        movies[0].Should().Be(movie2); // Highest rating
        movies[1].Should().Be(movie3); // Same rating as movie1, but newer
        movies[2].Should().Be(movie1); // Same rating as movie3, but older
    }

    [Fact]
    public void Movie_CompareTo_Null_ReturnsPositive()
    {
        // Arrange
        var movie = new Movie { Rating = 8.0 };

        // Act
        var result = movie.CompareTo(null);

        // Assert
        result.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Movie_CompareTo_SameRatingAndYear_ReturnsZero()
    {
        // Arrange
        var movie1 = new Movie { Rating = 8.0, ReleaseYear = 2020 };
        var movie2 = new Movie { Rating = 8.0, ReleaseYear = 2020 };

        // Act
        var result = movie1.CompareTo(movie2);

        // Assert
        result.Should().Be(0);
    }
}