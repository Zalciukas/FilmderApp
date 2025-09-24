using Filmder.DTOs;
using Filmder.Models;
using Filmder.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Filmder.Controllers;

[ApiController]
public class AccountController(UserManager<AppUser> userManager, SignInManager<AppUser>signInManager,ITokenService tokenService) : ControllerBase
{
    [HttpPost]
    [Route("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        var user = new AppUser
        {
            Email = registerDto.Email,
            UserName = registerDto.Email
        };
       var result = await userManager.CreateAsync(user, registerDto.Password);
       
       if (!result.Succeeded)
       {
           return BadRequest(result.Errors);
       }

       return new UserDto
       {
           Id = user.Id,
           Email = user.Email,
           Token = tokenService.CreateToken(user)
       };


    }

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
        {
            return Unauthorized("Invalid email");
        }
        var result = await signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
        if (!result.Succeeded) return Unauthorized("Invalid password");

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Token = tokenService.CreateToken(user)
        };
    }
    
}