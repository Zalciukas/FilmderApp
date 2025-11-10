using Filmder.Models;
using Filmder.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;

namespace Filmder.Tests.Services;

public class TokenServiceTests
{
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly TokenService _tokenService;

    public TokenServiceTests()
    {
        _mockConfig = new Mock<IConfiguration>();
        // Token key must be at least 64 characters
        _mockConfig.Setup(c => c["TokenKey"])
            .Returns("ThisIsAVeryLongSecretKeyThatIsAtLeast64CharactersLongForTestingPurposes!");
        
        _tokenService = new TokenService(_mockConfig.Object);
    }

    [Fact]
    public void CreateToken_ValidUser_ReturnsValidJwtToken()
    {
        // Arrange
        var user = new AppUser
        {
            Id = "user123",
            Email = "test@example.com",
            UserName = "testuser"
        };

        // Act
        var token = _tokenService.CreateToken(user);

        // Assert
        token.Should().NotBeNullOrEmpty();
        
        var handler = new JwtSecurityTokenHandler();
        handler.CanReadToken(token).Should().BeTrue();
        
        var jwtToken = handler.ReadJwtToken(token);
        
        jwtToken.Claims.Should().Contain(c => c.Value == user.Email, "Email should be in claims");
        jwtToken.Claims.Should().Contain(c => c.Value == user.Id, "User ID should be in claims");
        jwtToken.Claims.Should().Contain(c => c.Value == user.UserName, "Username should be in claims");
        
        // Token should have at least 3 claims
        Assert.True(jwtToken.Claims.Count() >= 3, "Token should have at least 3 claims");
    }

    [Fact]
    public void CreateToken_TokenExpiresInSevenDays()
    {
        // Arrange
        var user = new AppUser
        {
            Id = "user123",
            Email = "test@example.com",
            UserName = "testuser"
        };

        // Act
        var token = _tokenService.CreateToken(user);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        
        var expectedExpiry = DateTime.UtcNow.AddDays(7);
        jwtToken.ValidTo.Should().BeCloseTo(expectedExpiry, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public void CreateToken_NoTokenKeyInConfig_ThrowsException()
    {
        // Arrange
        var mockConfigNoKey = new Mock<IConfiguration>();
        mockConfigNoKey.Setup(c => c["TokenKey"]).Returns((string)null);
        var service = new TokenService(mockConfigNoKey.Object);
        
        var user = new AppUser
        {
            Id = "user123",
            Email = "test@example.com",
            UserName = "testuser"
        };

        // Act & Assert
        Assert.Throws<Exception>(() => service.CreateToken(user));
    }

    [Fact]
    public void CreateToken_TokenKeyTooShort_ThrowsException()
    {
        // Arrange
        var mockConfigShortKey = new Mock<IConfiguration>();
        mockConfigShortKey.Setup(c => c["TokenKey"]).Returns("shortkey");
        var service = new TokenService(mockConfigShortKey.Object);
        
        var user = new AppUser
        {
            Id = "user123",
            Email = "test@example.com",
            UserName = "testuser"
        };

        // Act & Assert
        Assert.Throws<Exception>(() => service.CreateToken(user));
    }
}