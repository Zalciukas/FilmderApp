using Filmder.DTOs;
using Filmder.Models;
using Microsoft.AspNetCore.Identity;

namespace Filmder.Services;

public class AccountService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService) : IAccountService
{
    public async Task<UserDto> RegisterAsync(RegisterDto registerDto)
    {
        var user = new AppUser
        {
            Email = registerDto.Email,
            UserName = registerDto.Email
        };
        var result = await userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));
        }
        return new UserDto(user.Id, user.Email!, tokenService.CreateToken(user));
    }

    public async Task<UserDto> LoginAsync(LoginDto loginDto)
    {
        var user = await userManager.FindByEmailAsync(loginDto.Email);
        if (user == null) throw new UnauthorizedAccessException("Invalid email");
        var result = await signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
        if (!result.Succeeded) throw new UnauthorizedAccessException("Invalid password");
        return new UserDto(user.Id, user.Email!, tokenService.CreateToken(user));
    }
}


