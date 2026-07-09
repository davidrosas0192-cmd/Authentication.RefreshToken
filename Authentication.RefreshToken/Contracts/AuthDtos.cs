namespace Authentication.RefreshToken.Contracts;

public record LoginRequest(string UserName, string Password);
public record AuthResponse(string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAt, DateTime RefreshTokenExpiresAt);
public record AccessTokenResponse(string AccessToken, DateTime AccessTokenExpiresAt);
public record RefreshTokenRequest(string RefreshToken);
public record LogoutRequest(string RefreshToken);
