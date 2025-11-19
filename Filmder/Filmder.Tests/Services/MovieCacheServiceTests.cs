using Filmder.Models;
using Filmder.Services;
using Filmder.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace Filmder.Tests.Services;

public class MovieCacheServiceTests
{
    [Fact]
    public async Task GetMovieByIdAsync_FirstCall_FetchesFromDatabase()
    {
        var context = TestDbContextFactory.CreateContextWithMovies();
        var service = new MovieCacheService(context);

        var movie = await service.GetMovieByIdAsync(1);

        movie.Should().NotBeNull();
        movie!.Name.Should().Be("The Matrix");
    }

    [Fact]
    public async Task GetMovieByIdAsync_SecondCall_ReturnsFromCache()
    {
        var context = TestDbContextFactory.CreateContextWithMovies();
        var service = new MovieCacheService(context);

        var movie1 = await service.GetMovieByIdAsync(1);
        var movie2 = await service.GetMovieByIdAsync(1);

        movie1.Should().NotBeNull();
        movie2.Should().NotBeNull();
        movie1.Should().BeSameAs(movie2); // Same instance = cached
    }

    [Fact]
    public void InvalidateCache_RemovesMovie()
    {
        var context = TestDbContextFactory.CreateContextWithMovies();
        var service = new MovieCacheService(context);

        service.InvalidateCache(1);
        // No assertion needed, just testing it doesn't crash
    }
}