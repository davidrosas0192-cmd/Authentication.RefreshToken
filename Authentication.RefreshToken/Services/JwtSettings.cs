namespace Authentication.RefreshToken.Services;

public class JwtSettings
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = "Authentication.RefreshToken";
    public string Audience { get; set; } = "Authentication.RefreshToken.Client";
    public int AccessTokenMinutes { get; set; } = 15;
    public int RefreshTokenMinutes { get; set; } = 60 * 24 * 7;
}
