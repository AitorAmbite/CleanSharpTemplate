namespace Template.Api.Features.Auth;

using Template.Api.Features.Auth.Models;
using Template.Application.Features.Auth.Commands.Login;
using Template.Application.Features.Auth.Commands.RefreshToken;
using Template.Application.Features.Auth.Commands.Register;
using Template.Application.Features.Auth.Queries.GetCurrentUser;
using Template.Contracts.Auth;
using Wolverine;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/register", RegisterAsync)
            .WithName("Register")
            .AllowAnonymous();

        app.MapPost("/auth/login", LoginAsync)
            .WithName("Login")
            .AllowAnonymous();

        app.MapPost("/auth/refresh", RefreshTokenAsync)
            .WithName("RefreshToken")
            .AllowAnonymous();

        app.MapGet("/auth/me", GetCurrentUserAsync)
            .WithName("GetCurrentUser")
            .RequireAuthorization();

        return app;
    }

    private static async Task<TokenResponse> RegisterAsync(
        RegisterRequest request,
        IMessageBus bus,
        CancellationToken cancellationToken = default)
    {
        var command = new RegisterCommand(request.Username, request.Email, request.Password);
        return await bus.InvokeAsync<TokenResponse>(command, cancellationToken);
    }

    private static async Task<TokenResponse> LoginAsync(
        LoginRequest request,
        IMessageBus bus,
        CancellationToken cancellationToken = default)
    {
        var command = new LoginCommand(request.UsernameOrEmail, request.Password);
        return await bus.InvokeAsync<TokenResponse>(command, cancellationToken);
    }

    private static async Task<TokenResponse> RefreshTokenAsync(
        RefreshTokenRequest request,
        IMessageBus bus,
        CancellationToken cancellationToken = default)
    {
        var command = new RefreshTokenCommand(request.RefreshToken);
        return await bus.InvokeAsync<TokenResponse>(command, cancellationToken);
    }

    private static async Task<CurrentUser> GetCurrentUserAsync(
        IMessageBus bus,
        CancellationToken cancellationToken = default)
    {
        return await bus.InvokeAsync<CurrentUser>(new GetCurrentUserQuery(), cancellationToken);
    }
}
