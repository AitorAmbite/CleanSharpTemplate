namespace Template.Domain.Entities;

using Template.Domain.Common;

public class RefreshToken : Entity
{
    public Guid UserId { get; private set; }

    public string Token { get; private set; } = string.Empty;

    public DateTime ExpiresAt { get; private set; }

    public bool IsRevoked { get; private set; }

    public string? ReplacedByToken { get; private set; }

    public bool IsActive => !IsRevoked && ExpiresAt > DateTime.UtcNow;

    public static RefreshToken Create(Guid userId, string token, DateTime expiresAt)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id is required.", nameof(userId));
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Token is required.", nameof(token));
        }

        return new RefreshToken
        {
            UserId = userId,
            Token = token,
            ExpiresAt = expiresAt,
        };
    }

    public void Revoke(string? replacedByToken = null)
    {
        IsRevoked = true;
        ReplacedByToken = replacedByToken;
    }
}
