namespace Authentication.RefreshToken.Contracts;

public record LoginRequest(string UserName, string Password);
public record AuthResponse(string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAt, DateTime RefreshTokenExpiresAt);
public record RefreshTokenRequest(string RefreshToken);
public record LogoutRequest(string RefreshToken);
