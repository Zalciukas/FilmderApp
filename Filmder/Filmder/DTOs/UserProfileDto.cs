using Filmder.Models;

namespace Filmder.DTOs;

public class UserProfileDto
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string? ProfilePictureUrl { get; set; }
}