using Filmder.Extensions;
using Filmder.Models;
using FluentAssertions;
using Xunit;

namespace Filmder.Tests.Extensions;

public class MovieGenreParsingExtensionsTests
{
    [Theory]
    [InlineData("action", MovieGenre.Action)]
    [InlineData("Action", MovieGenre.Action)]
    [InlineData("ACTION", MovieGenre.Action)]
    [InlineData("comedy", MovieGenre.Comedy)]
    [InlineData("drama", MovieGenre.Drama)]
    [InlineData("horror", MovieGenre.Horror)]
    [InlineData("thriller", MovieGenre.Thriller)]
    [InlineData("romance", MovieGenre.Romance)]
    [InlineData("romantic", MovieGenre.Romance)]
    [InlineData("scifi", MovieGenre.SciFi)]
    [InlineData("sci-fi", MovieGenre.SciFi)]
    [InlineData("sciencefiction", MovieGenre.SciFi)]
    [InlineData("animation", MovieGenre.Animation)]
    [InlineData("animated", MovieGenre.Animation)]
    [InlineData("documentary", MovieGenre.Documentary)]
    [InlineData("doc", MovieGenre.Documentary)]
    [InlineData("docu", MovieGenre.Documentary)]
    [InlineData("biography", MovieGenre.Biography)]
    [InlineData("biopic", MovieGenre.Biography)]
    [InlineData("bio", MovieGenre.Biography)]
    [InlineData("crime", MovieGenre.Crime)]
    [InlineData("criminal", MovieGenre.Crime)]
    [InlineData("adventure", MovieGenre.Adventure)]
    [InlineData("family", MovieGenre.Family)]
    [InlineData("fantasy", MovieGenre.Fantasy)]
    [InlineData("filmnoir", MovieGenre.FilmNoir)]
    [InlineData("noir", MovieGenre.FilmNoir)]
    [InlineData("history", MovieGenre.History)]
    [InlineData("historical", MovieGenre.History)]
    [InlineData("music", MovieGenre.Music)]
    [InlineData("musical", MovieGenre.Musical)]
    [InlineData("mystery", MovieGenre.Mystery)]
    [InlineData("mysteries", MovieGenre.Mystery)]
    [InlineData("sport", MovieGenre.Sport)]
    [InlineData("sports", MovieGenre.Sport)]
    [InlineData("superhero", MovieGenre.Superhero)]
    [InlineData("superheroes", MovieGenre.Superhero)]
    [InlineData("war", MovieGenre.War)]
    [InlineData("warfare", MovieGenre.War)]
    [InlineData("western", MovieGenre.Western)]
    public void TryParseGenre_ValidGenre_ReturnsTrue(string input, MovieGenre expected)
    {
        // Act
        var result = input.TryParseGenre(out var genre);

        // Assert
        result.Should().BeTrue();
        genre.Should().Be(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("invalid")]
    [InlineData("notAGenre")]
    [InlineData("123")]
    [InlineData("@#$%")]
    public void TryParseGenre_InvalidGenre_ReturnsFalse(string input)
    {
        // Act
        var result = input.TryParseGenre(out var genre);

        // Assert
        result.Should().BeFalse();
        genre.Should().Be(default(MovieGenre));
    }

    [Theory]
    [InlineData("Action ", MovieGenre.Action)]
    [InlineData(" Comedy", MovieGenre.Comedy)]
    [InlineData(" Drama ", MovieGenre.Drama)]
    [InlineData("Sci-Fi", MovieGenre.SciFi)]
    [InlineData("Sci_Fi", MovieGenre.SciFi)]
    [InlineData("Film Noir", MovieGenre.FilmNoir)]
    public void TryParseGenre_GenreWithWhitespaceOrSpecialChars_HandlesCorrectly(string input, MovieGenre expected)
    {
        // Act
        var result = input.TryParseGenre(out var genre);

        // Assert
        result.Should().BeTrue();
        genre.Should().Be(expected);
    }
}