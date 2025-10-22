using System.Text.Json;
using Filmder.Data;
using Filmder.Models;

namespace Filmder.Services
{
    public class MovieImportService
    {
        private readonly AppDbContext _context;
        private readonly string _filePath = "movies.json";

        public MovieImportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> ImportMoviesFromFileAsync(string filePath = "movies.json")
        {
            if (!File.Exists(filePath))
                return 0;

            using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var movies = await JsonSerializer.DeserializeAsync<List<Movie>>(stream);

            if (movies == null || movies.Count == 0)
                return 0;

            foreach (var movie in movies)
            {
                if (!_context.Movies.Any(m => m.Name == movie.Name && m.ReleaseYear == movie.ReleaseYear))
                {
                    _context.Movies.Add(movie);
                }
            }

            return await _context.SaveChangesAsync();
        }

    }
}