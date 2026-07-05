namespace Template.Application.Features.Auth.Queries.GetCurrentUser;

using Template.Application.Abstractions;
using Template.Contracts.Auth;

public class GetCurrentUserQueryHandler
{
    private readonly ICurrentUserService _currentUserService;

    public GetCurrentUserQueryHandler(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public Task<CurrentUser> Handle(GetCurrentUserQuery query, CancellationToken cancellationToken)
    {
        return Task.FromResult(_currentUserService.GetCurrentUser());
    }
}
