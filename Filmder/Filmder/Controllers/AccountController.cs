using Filmder.DTOs;
using Filmder.Models;
using Filmder.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Filmder.Controllers;

[ApiController]
public class AccountController(IAccountService accountService) : ControllerBase
{
    [HttpPost]
    [Route("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        try
        {
            var userDto = await accountService.RegisterAsync(registerDto);
            return userDto;
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new[] { new { Description = ex.Message } });
        }

    }

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        try
        {
            var user = await accountService.LoginAsync(loginDto);
            return user;
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        
    }
    
}