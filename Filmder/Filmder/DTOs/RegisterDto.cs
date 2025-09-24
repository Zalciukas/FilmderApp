using System.ComponentModel.DataAnnotations;

namespace Filmder.DTOs;

public class RegisterDto
{
    [Required] public string Email { get; set; } = "";
    [Required] [MinLength(4)] public string Password { get; set; } = "";
}