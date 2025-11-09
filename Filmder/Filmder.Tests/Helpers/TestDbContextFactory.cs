using Filmder.Data;
using Filmder.Models;
using Microsoft.EntityFrameworkCore;

namespace Filmder.Tests.Helpers;

public static class TestDbContextFactory
{
    public static AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        return context;
    }

    public static AppDbContext CreateContextWithMovies()
    {
        var context = CreateInMemoryContext();
        
        context.Movies.AddRange(
            new Movie
            {
                Id = 1,
                Name = "The Matrix",
                Genre = MovieGenre.SciFi,
                Description = "People living in a simulation",
                ReleaseYear = 1999,
                Rating = 8.7,
                Duration = 136,
                Director = "Wachowski Brothers",
                Cast = "Keanu Reeves, Laurence Fishburne",
                PosterUrl = "https://example.com/matrix.jpg",
                TrailerUrl = "https://example.com/matrix-trailer.mp4"
            },
            new Movie
            {
                Id = 2,
                Name = "Inception",
                Genre = MovieGenre.SciFi,
                Description = "People with schizofrenia doing stuff.",
                ReleaseYear = 2010,
                Rating = 8.8,
                Duration = 148,
                Director = "Christopher Nolan",
                Cast = "Leonardo DiCaprio, Tom Hardy"
            },
            new Movie
            {
                Id = 3,
                Name = "The Godfather",
                Genre = MovieGenre.Crime,
                Description = "Old movie",
                ReleaseYear = 1972,
                Rating = 9.2,
                Duration = 175,
                Director = "Francis Ford Coppola",
                Cast = "Marlon Brando, Al Pacino"
            },
            new Movie
            {
                Id = 4,
                Name = "Toy Story",
                Genre = MovieGenre.Animation,
                Description = "Best kids movie",
                ReleaseYear = 1995,
                Rating = 8.3,
                Duration = 81,
                Director = "John Lasseter",
                Cast = "Tom Hanks, Tim Allen"
            }
        );
        
        context.SaveChanges();
        return context;
    }

    public static AppDbContext CreateContextWithUsers()
    {
        var context = CreateInMemoryContext();
        
        context.Users.AddRange(
            new AppUser
            {
                Id = "user1",
                Email = "test1@example.com",
                UserName = "test1@example.com",
                FavoriteGenre = "Action"
            },
            new AppUser
            {
                Id = "user2",
                Email = "test2@example.com",
                UserName = "test2@example.com",
                FavoriteGenre = "Comedy"
            }
        );
        
        context.SaveChanges();
        return context;
    }
}