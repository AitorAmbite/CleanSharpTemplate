namespace Template.Application.Abstractions;

using Template.Contracts.Auth;

public interface ICurrentUserService
{
    CurrentUser GetCurrentUser();
}
