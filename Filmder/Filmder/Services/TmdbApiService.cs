using Filmder.DTOs;
using Filmder.Models;
using Microsoft.Extensions.Configuration;

public class TmdbApiService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private const string ImageBase = "https://image.tmdb.org/t/p/w500";

    public TmdbApiService(IConfiguration config)
    {
        _apiKey = config["TmdbApiKey"] 
            ?? throw new Exception("TMDB API key missing from appsettings.json!");

        _http = new HttpClient
        {
            BaseAddress = new Uri("https://api.themoviedb.org/3/")
        };
    }

    private List<SimpleMovieDto> MapMovies(List<TmdbMovie> movies)
    {
        return movies
            .Where(m => !string.IsNullOrEmpty(m.title))
            .Select(m => new SimpleMovieDto
            {
                Title = m.title,
                ReleaseDate = m.release_date,
                Overview = m.overview,
                PosterUrl = m.poster_path == null ? null : $"{ImageBase}{m.poster_path}"
            })
            .ToList();
    }

    private PagedMoviesResponseDto MapResponse(TmdbPagedResponse tmdb)
    {
        return new PagedMoviesResponseDto
        {
            Page = tmdb.page,
            TotalPages = tmdb.total_pages,
            Movies = MapMovies(tmdb.results)
        };
    }

    public async Task<PagedMoviesResponseDto> GetTrendingMovies(int page = 1)
    {
        var resp = await _http.GetFromJsonAsync<TmdbPagedResponse>(
            $"trending/movie/week?api_key={_apiKey}&page={page}"
        );
        return MapResponse(resp);
    }

    public async Task<PagedMoviesResponseDto> GetPopularMovies(int page = 1)
    {
        var resp = await _http.GetFromJsonAsync<TmdbPagedResponse>(
            $"movie/popular?api_key={_apiKey}&page={page}"
        );
        return MapResponse(resp);
    }

    public async Task<PagedMoviesResponseDto> GetTopRatedMovies(int page = 1)
    {
        var resp = await _http.GetFromJsonAsync<TmdbPagedResponse>(
            $"movie/top_rated?api_key={_apiKey}&page={page}"
        );
        return MapResponse(resp);
    }

    public async Task<PagedMoviesResponseDto> GetUpcomingMovies(int page = 1)
    {
        var resp = await _http.GetFromJsonAsync<TmdbPagedResponse>(
            $"movie/upcoming?api_key={_apiKey}&page={page}"
        );
        return MapResponse(resp);
    }
}
