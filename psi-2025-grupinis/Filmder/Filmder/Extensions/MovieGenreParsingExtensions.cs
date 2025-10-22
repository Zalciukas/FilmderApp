using Filmder.Models;

namespace Filmder.Extensions;

public static class MovieGenreParsingExtensions
{
	public static bool TryParseGenre(string? value, out MovieGenre genre)
	{
		genre = default;
		if (string.IsNullOrWhiteSpace(value)) return false;
		var normalized = value.Trim().Replace(" ", "").Replace("-", "").Replace("_", "").ToLowerInvariant();
		switch (normalized)
		{
			case "action": genre = MovieGenre.Action; return true;
			case "comedy": genre = MovieGenre.Comedy; return true;
			case "drama": genre = MovieGenre.Drama; return true;
			case "horror": genre = MovieGenre.Horror; return true;
			case "scifi":
			case "sciencefiction": genre = MovieGenre.SciFi; return true;
			case "romance": genre = MovieGenre.Romance; return true;
			case "thriller": genre = MovieGenre.Thriller; return true;
			case "animation": genre = MovieGenre.Animation; return true;
			case "documentary": genre = MovieGenre.Documentary; return true;
			case "fantasy": genre = MovieGenre.Fantasy; return true;
			default:
				return false;
		}
	}
}


