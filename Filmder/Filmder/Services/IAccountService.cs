using Filmder.DTOs;
using Filmder.Models;

namespace Filmder.Services;

public interface IAccountService
{
    Task<UserDto> RegisterAsync(RegisterDto registerDto);
    Task<UserDto> LoginAsync(LoginDto loginDto);
}


