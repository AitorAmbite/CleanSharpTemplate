namespace Template.Application.Abstractions;

using Template.Domain.Entities;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);

    string GenerateRefreshToken();

    Guid? GetUserIdFromExpiredToken(string token);
}
