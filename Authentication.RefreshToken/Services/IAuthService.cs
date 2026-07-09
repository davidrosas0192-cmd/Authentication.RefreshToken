using Authentication.RefreshToken.Contracts;
using Authentication.RefreshToken.Models;

namespace Authentication.RefreshToken.Services;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
    Task<AuthResponse> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken);
    Task LogoutAsync(LogoutRequest request, CancellationToken cancellationToken);
    Task<User?> GetUserByUserNameAsync(string userName, CancellationToken cancellationToken);
}
