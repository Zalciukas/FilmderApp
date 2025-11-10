using Filmder.Controllers;
using Filmder.DTOs;
using Filmder.Models;
using Filmder.Services;
using Filmder.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Filmder.Tests.Controllers;

public class AccountControllerTests
{
    private readonly Mock<UserManager<AppUser>> _mockUserManager;
    private readonly Mock<SignInManager<AppUser>> _mockSignInManager;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly AccountController _controller;

    public AccountControllerTests()
    {
        _mockUserManager = MockHelpers.GetMockUserManager();
        _mockSignInManager = MockHelpers.GetMockSignInManager(_mockUserManager);
        _mockTokenService = new Mock<ITokenService>();
        
        _controller = new AccountController(
            _mockUserManager.Object,
            _mockSignInManager.Object,
            _mockTokenService.Object
        );
    }

    [Fact]
    public async Task Register_ValidDto_ReturnsUserDto()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Email = "test@example.com",
            Password = "Password123"
        };

        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        _mockTokenService.Setup(x => x.CreateToken(It.IsAny<AppUser>()))
            .Returns("fake-jwt-token");

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        var actionResult = result.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        
        var userDto = actionResult.Value as UserDto;
        userDto.Should().NotBeNull();
        userDto.Email.Should().Be(registerDto.Email);
        userDto.Token.Should().Be("fake-jwt-token");
    }
    
    [Fact]
    public async Task Register_CreationFails_ReturnsBadRequest()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Email = "test@example.com",
            Password = "weak"
        };

        var errors = new[]
        {
            new IdentityError { Code = "PasswordTooShort", Description = "Password is too short" }
        };

        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(errors));

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        var actionResult = result.Result as BadRequestObjectResult;
        actionResult.Should().NotBeNull();
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsUserDto()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "Password123"
        };

        var user = new AppUser
        {
            Id = "user123",
            Email = loginDto.Email,
            UserName = loginDto.Email
        };
        
        _mockUserManager.Setup(x => x.FindByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);

        _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(user, loginDto.Password, false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        _mockTokenService.Setup(x => x.CreateToken(user))
            .Returns("fake-jwt-token");
        
        //Act
        var result  = await _controller.Login(loginDto);
        
        // Assert
        var actionResult = result.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        
        var userDto = actionResult.Value as UserDto;
        userDto.Should().NotBeNull();
        userDto.Email.Should().Be(loginDto.Email);
        userDto.Token.Should().Be("fake-jwt-token");
    }
    
    [Fact]
    public async Task Login_UserNotFound_ReturnsUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "nonexistent@example.com",
            Password = "Password123"
        };

        _mockUserManager.Setup(x => x.FindByEmailAsync(loginDto.Email))
            .ReturnsAsync((AppUser)null);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        var actionResult = result.Result as UnauthorizedObjectResult;
        actionResult.Should().NotBeNull();
        actionResult.Value.Should().Be("Invalid email");
    }

    [Fact]
    public async Task Login_InvalidPassword_ReturnsUnauthorizes()
    {
        //Arrange
        var loginDto = new LoginDto
        {
            Email = "test1@example.com",
            Password = "WRONGPASWWORD"
        };

        var user = new AppUser
        {
            Id = "user123",
            Email = loginDto.Email,
            UserName = loginDto.Email
        };
        
        _mockUserManager.Setup(x => x.FindByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);

        _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(user, loginDto.Password, false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);
        
        // Act
        var result = await _controller.Login(loginDto);
        
        // Assert
        var actionResult = result.Result as UnauthorizedObjectResult;
        actionResult.Should().NotBeNull();
        actionResult.Value.Should().Be("Invalid password");
    }
}