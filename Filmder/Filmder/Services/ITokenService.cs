using Filmder.Models;

namespace Filmder.Services;

public interface ITokenService
{
    String CreateToken(AppUser user);
}