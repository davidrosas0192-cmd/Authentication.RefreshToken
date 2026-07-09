using System.Security.Cryptography;
using System.Text;
using Authentication.RefreshToken.Contracts;
using Authentication.RefreshToken.Data;
using Authentication.RefreshToken.Models;
using Microsoft.EntityFrameworkCore;

namespace Authentication.RefreshToken.Services;

public class AuthService(ApplicationDbContext context, ITokenService tokenService, IConfiguration configuration) : IAuthService
{
    private readonly JwtSettings _jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>() ?? new JwtSettings();
    private readonly string _refreshTokenHashingKey = configuration["RefreshTokenHashingKey"] ?? throw new InvalidOperationException("RefreshTokenHashingKey is not configured.");

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await context.Users.SingleOrDefaultAsync(u => u.UserName == request.UserName, cancellationToken);
        if (user is null)
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        if (!VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        await RevokeExistingSessionsAsync(user.Id, "Replaced by new login", cancellationToken);

        var refreshToken = tokenService.CreateRefreshToken();
        var refreshTokenHash = HashToken(refreshToken);
        var session = new UserSession
        {
            UserId = user.Id,
            RefreshTokenHash = refreshTokenHash,
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.RefreshTokenMinutes),
            IsActive = true
        };

        context.UserSessions.Add(session);
        await context.SaveChangesAsync(cancellationToken);

        user.LastLoginAt = DateTime.UtcNow;
        await context.SaveChangesAsync(cancellationToken);

        return new AuthResponse(
            tokenService.CreateAccessToken(user),
            refreshToken,
            DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenMinutes),
            session.ExpiresAt);
    }

    public async Task<AuthResponse> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var session = await GetActiveSessionAsync(request.RefreshToken, cancellationToken);
        if (session is null)
        {
            throw new UnauthorizedAccessException("Refresh token is invalid or expired.");
        }

        await RevokeExistingSessionsAsync(session.UserId, "Rotated refresh token", cancellationToken);

        var newRefreshToken = tokenService.CreateRefreshToken();
        var newRefreshTokenHash = HashToken(newRefreshToken);
        var newSession = new UserSession
        {
            UserId = session.UserId,
            RefreshTokenHash = newRefreshTokenHash,
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.RefreshTokenMinutes),
            IsActive = true
        };

        context.UserSessions.Add(newSession);
        await context.SaveChangesAsync(cancellationToken);

        return new AuthResponse(
            tokenService.CreateAccessToken(session.User),
            newRefreshToken,
            DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenMinutes),
            newSession.ExpiresAt);
    }

    public async Task<AccessTokenResponse> RefreshAccessTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var session = await GetActiveSessionAsync(request.RefreshToken, cancellationToken);
        if (session is null)
        {
            throw new UnauthorizedAccessException("Refresh token is invalid or expired.");
        }

        return new AccessTokenResponse(
            tokenService.CreateAccessToken(session.User),
            DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenMinutes));
    }

    public async Task LogoutAsync(LogoutRequest request, CancellationToken cancellationToken)
    {
        var refreshTokenHash = HashToken(request.RefreshToken);
        var session = await context.UserSessions.SingleOrDefaultAsync(s => s.RefreshTokenHash == refreshTokenHash && s.IsActive, cancellationToken);
        if (session is null)
        {
            return;
        }

        session.IsActive = false;
        session.RevokedAt = DateTime.UtcNow;
        session.RevokedReason = "User logout";
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task<User?> GetUserByUserNameAsync(string userName, CancellationToken cancellationToken)
        => context.Users.AsNoTracking().SingleOrDefaultAsync(u => u.UserName == userName, cancellationToken);

    private async Task<UserSession?> GetActiveSessionAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var refreshTokenHash = HashToken(refreshToken);
        var session = await context.UserSessions
            .Include(s => s.User)
            .SingleOrDefaultAsync(s => s.RefreshTokenHash == refreshTokenHash && s.IsActive, cancellationToken);

        if (session is null || session.ExpiresAt <= DateTime.UtcNow || session.RevokedAt.HasValue)
        {
            return null;
        }

        return session;
    }

    private async Task RevokeExistingSessionsAsync(int userId, string reason, CancellationToken cancellationToken)
    {
        var sessions = await context.UserSessions
            .Where(s => s.UserId == userId && s.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var session in sessions)
        {
            session.IsActive = false;
            session.RevokedAt = DateTime.UtcNow;
            session.RevokedReason = reason;
        }

        if (sessions.Count > 0)
        {
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    private string HashToken(string token)
    {
        var key = Encoding.UTF8.GetBytes(_refreshTokenHashingKey);
        using var hmac = new HMACSHA256(key);
        var bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }

    private static bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
