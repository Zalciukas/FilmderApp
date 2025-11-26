using Filmder.Models;

namespace Filmder.DTOs;

public class TmdbPagedResponse
{
    public int page { get; set; }
    public int total_pages { get; set; }
    public List<TmdbMovie> results { get; set; }
}