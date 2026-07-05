namespace Template.Infrastructure.Services;

using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Template.Application.Abstractions;
using Template.Contracts.Auth;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public CurrentUser GetCurrentUser()
    {
        ClaimsPrincipal? user = _httpContextAccessor.HttpContext?.User;

        if (user?.Identity?.IsAuthenticated != true)
        {
            return new CurrentUser(Guid.Empty, string.Empty, string.Empty, false);
        }

        string? userIdValue = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? user.FindFirst("sub")?.Value;
        string? userName = user.FindFirst(ClaimTypes.Name)?.Value
            ?? user.FindFirst("name")?.Value;
        string? email = user.FindFirst(ClaimTypes.Email)?.Value
            ?? user.FindFirst("email")?.Value;

        if (!Guid.TryParse(userIdValue, out Guid userId))
        {
            return new CurrentUser(Guid.Empty, string.Empty, string.Empty, false);
        }

        return new CurrentUser(userId, userName ?? string.Empty, email ?? string.Empty, true);
    }
}
