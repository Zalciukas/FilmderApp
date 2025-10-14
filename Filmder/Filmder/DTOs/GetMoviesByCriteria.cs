namespace Filmder.DTOs;

public class GetMoviesByCriteriaDto
{
    public int MovieCount { get; set; }        
    public string? Genre { get; set; }          
    public int? LongestDurationMinutes { get; set; }  
    public int ReleaseDate { get; set; } 
    
}
