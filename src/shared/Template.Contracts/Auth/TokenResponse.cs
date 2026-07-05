namespace Template.Contracts.Auth;

public record TokenResponse(string AccessToken, string RefreshToken, int ExpiresIn, string TokenType);
