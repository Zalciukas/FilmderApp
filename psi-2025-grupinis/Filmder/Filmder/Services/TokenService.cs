using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Filmder.Models;
using Microsoft.IdentityModel.Tokens;

namespace Filmder.Services;

public class TokenService(IConfiguration config) : ITokenService
{

    public String CreateToken(AppUser user)
    {
        var tokenKey = config["TokenKey"] ?? throw new Exception("no key found");
        if (tokenKey.Length < 64) throw new Exception("Invalid token key");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name,user.UserName)
        };
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);    
    }
}