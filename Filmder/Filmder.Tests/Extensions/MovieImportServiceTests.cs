using Filmder.Models;
using Filmder.Services;
using Filmder.Tests.Helpers;
using FluentAssertions;
using System.Text.Json;
using Xunit;

namespace Filmder.Tests.Services;

public class MovieImportServiceTests : IDisposable
{
    private readonly string _testFilePath = "test_movies.json";

    [Fact]
    public async Task ImportMoviesFromFileAsync_FileDoesNotExist_ReturnsZero()
    {
        // Arrange
        var context = TestDbContextFactory.CreateInMemoryContext();
        var service = new MovieImportService(context);

        // Act
        var result = await service.ImportMoviesFromFileAsync("nonexistent.json");

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task ImportMoviesFromFileAsync_EmptyFile_ReturnsZero()
    {
        // Arrange
        var context = TestDbContextFactory.CreateInMemoryContext();
        var service = new MovieImportService(context);
        
        await File.WriteAllTextAsync(_testFilePath, "[]");

        // Act
        var result = await service.ImportMoviesFromFileAsync(_testFilePath);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task ImportMoviesFromFileAsync_ValidMovies_ImportsSuccessfully()
    {
        // Arrange
        var context = TestDbContextFactory.CreateInMemoryContext();
        var service = new MovieImportService(context);
        
        var movies = new List<Movie>
        {
            new Movie
            {
                Name = "Test Movie 1",
                Genre = MovieGenre.Action,
                Description = "Test description",
                ReleaseYear = 2020,
                Rating = 8.0,
                Duration = 120,
                Director = "Test Director",
                Cast = "Actor 1, Actor 2"
            },
            new Movie
            {
                Name = "Test Movie 2",
                Genre = MovieGenre.Comedy,
                Description = "Test description 2",
                ReleaseYear = 2021,
                Rating = 7.5,
                Duration = 110,
                Director = "Test Director 2",
                Cast = "Actor 3, Actor 4"
            }
        };
        
        var json = JsonSerializer.Serialize(movies);
        await File.WriteAllTextAsync(_testFilePath, json);

        // Act
        var result = await service.ImportMoviesFromFileAsync(_testFilePath);

        // Assert
        result.Should().Be(2);
        context.Movies.Should().HaveCount(2);
    }

    [Fact]
    public async Task ImportMoviesFromFileAsync_DuplicateMovies_SkipsDuplicates()
    {
        // Arrange
        var context = TestDbContextFactory.CreateInMemoryContext();
        
        // Add existing movie
        context.Movies.Add(new Movie
        {
            Name = "Existing Movie",
            Genre = MovieGenre.Drama,
            ReleaseYear = 2020,
            Rating = 8.0,
            Duration = 120,
            Director = "Director",
            Cast = "Cast"
        });
        await context.SaveChangesAsync();
        
        var service = new MovieImportService(context);
        
        var movies = new List<Movie>
        {
            new Movie
            {
                Name = "Existing Movie",
                Genre = MovieGenre.Drama,
                ReleaseYear = 2020,
                Rating = 8.0,
                Duration = 120,
                Director = "Director",
                Cast = "Cast"
            },
            new Movie
            {
                Name = "New Movie",
                Genre = MovieGenre.Action,
                ReleaseYear = 2021,
                Rating = 7.0,
                Duration = 100,
                Director = "New Director",
                Cast = "New Cast"
            }
        };
        
        var json = JsonSerializer.Serialize(movies);
        await File.WriteAllTextAsync(_testFilePath, json);

        // Act
        var result = await service.ImportMoviesFromFileAsync(_testFilePath);

        // Assert
        result.Should().Be(1); // Only 1 new movie added
        context.Movies.Should().HaveCount(2); // Total of 2 movies
    }

    public void Dispose()
    {
        if (File.Exists(_testFilePath))
        {
            File.Delete(_testFilePath);
        }
    }
}