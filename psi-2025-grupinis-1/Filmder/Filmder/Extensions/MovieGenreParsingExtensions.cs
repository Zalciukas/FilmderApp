using Filmder.Models;

namespace Filmder.Extensions;

public static class MovieGenreParsingExtensions
{
    public static bool TryParseGenre(this string? value, out MovieGenre genre)
    {
        genre = default;
        if (string.IsNullOrWhiteSpace(value)) return false;
        
        var normalized = value.Trim().Replace(" ", "").Replace("-", "").Replace("_", "").ToLowerInvariant();
        
        switch (normalized)
        {
            case "action": 
                genre = MovieGenre.Action; 
                return true;
            case "adventure": 
                genre = MovieGenre.Adventure; 
                return true;
            case "animation":
            case "animated": 
                genre = MovieGenre.Animation; 
                return true;
            case "biography":
            case "biopic":
            case "bio": 
                genre = MovieGenre.Biography; 
                return true;
            case "comedy": 
                genre = MovieGenre.Comedy; 
                return true;
            case "crime":
            case "criminal": 
                genre = MovieGenre.Crime; 
                return true;
            case "documentary":
            case "doc":
            case "docu": 
                genre = MovieGenre.Documentary; 
                return true;
            case "drama": 
                genre = MovieGenre.Drama; 
                return true;
            case "family": 
                genre = MovieGenre.Family; 
                return true;
            case "fantasy": 
                genre = MovieGenre.Fantasy; 
                return true;
            case "filmnoir":
            case "noir": 
                genre = MovieGenre.FilmNoir; 
                return true;
            case "history":
            case "historical": 
                genre = MovieGenre.History; 
                return true;
            case "horror": 
                genre = MovieGenre.Horror; 
                return true;
            case "music": 
                genre = MovieGenre.Music; 
                return true;
            case "musical": 
                genre = MovieGenre.Musical; 
                return true;
            case "mystery":
            case "mysteries": 
                genre = MovieGenre.Mystery; 
                return true;
            case "romance":
            case "romantic": 
                genre = MovieGenre.Romance; 
                return true;
            case "scifi":
            case "sciencefiction":
            case "sci-fi": 
                genre = MovieGenre.SciFi; 
                return true;
            case "sport":
            case "sports": 
                genre = MovieGenre.Sport; 
                return true;
            case "superhero":
            case "superheroes": 
                genre = MovieGenre.Superhero; 
                return true;
            case "thriller": 
                genre = MovieGenre.Thriller; 
                return true;
            case "war":
            case "warfare": 
                genre = MovieGenre.War; 
                return true;
            case "western": 
                genre = MovieGenre.Western; 
                return true;
            default:
                return false;
        }
    }
}