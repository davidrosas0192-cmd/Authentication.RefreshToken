using Authentication.RefreshToken.Models;

namespace Authentication.RefreshToken.Services;

public interface ITokenService
{
    string CreateAccessToken(User user);
    string CreateRefreshToken();
}
