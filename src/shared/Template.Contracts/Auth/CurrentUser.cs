namespace Template.Contracts.Auth;

public record CurrentUser(Guid UserId, string UserName, string Email, bool IsAuthenticated);
