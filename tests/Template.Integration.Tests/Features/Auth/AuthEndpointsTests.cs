namespace Template.Integration.Tests.Features.Auth;

using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Template.Api.Features.Auth.Models;
using Template.Contracts.Auth;
using Template.Infrastructure.Persistence;
using Template.Integration.Tests.Fixtures;
using Testcontainers.PostgreSql;

public class AuthEndpointsTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres;
    private readonly CustomWebApplicationFactory _factory;

    public AuthEndpointsTests()
    {
        _postgres = new PostgreSqlBuilder("postgres:16-alpine")
            .WithDatabase("template")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        _factory = new CustomWebApplicationFactory();
    }

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        _factory.ConnectionString = _postgres.GetConnectionString();

        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    [Fact]
    public async Task Register_Returns_TokenResponse_With_Valid_Token()
    {
        var client = _factory.CreateClient();
        var request = new RegisterRequest("testuser", "test@example.com", "Password123!");

        var response = await client.PostAsJsonAsync("/auth/register", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tokens = await response.Content.ReadFromJsonAsync<TokenResponse>();
        Assert.NotNull(tokens);
        Assert.Equal("Bearer", tokens!.TokenType);
        Assert.NotEmpty(tokens.AccessToken);
        Assert.NotEmpty(tokens.RefreshToken);
        Assert.True(tokens.ExpiresIn > 0);

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(tokens.AccessToken);
        Assert.Equal("testuser", token.Claims.First(c => c.Type == "name").Value);
        Assert.NotNull(token.Claims.First(c => c.Type == "sub").Value);
    }

    [Fact]
    public async Task Register_With_Duplicate_Email_Returns_BadRequest()
    {
        var client = _factory.CreateClient();
        var request = new RegisterRequest("user1", "duplicate@example.com", "Password123!");
        await client.PostAsJsonAsync("/auth/register", request);

        var duplicateRequest = new RegisterRequest("user2", "duplicate@example.com", "Password123!");
        var response = await client.PostAsJsonAsync("/auth/register", duplicateRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_With_Valid_Credentials_Returns_TokenResponse()
    {
        var client = _factory.CreateClient();
        var registerRequest = new RegisterRequest("loginuser", "login@example.com", "Password123!");
        await client.PostAsJsonAsync("/auth/register", registerRequest);

        var loginRequest = new LoginRequest("loginuser", "Password123!");
        var response = await client.PostAsJsonAsync("/auth/login", loginRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tokens = await response.Content.ReadFromJsonAsync<TokenResponse>();
        Assert.NotNull(tokens);
        Assert.NotEmpty(tokens!.AccessToken);
        Assert.NotEmpty(tokens.RefreshToken);
    }

    [Fact]
    public async Task Login_With_Invalid_Credentials_Returns_BadRequest()
    {
        var client = _factory.CreateClient();
        var loginRequest = new LoginRequest("nonexistent", "wrongpassword");

        var response = await client.PostAsJsonAsync("/auth/login", loginRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_With_Valid_Token_Returns_New_Tokens()
    {
        var client = _factory.CreateClient();
        var registerRequest = new RegisterRequest("refreshuser", "refresh@example.com", "Password123!");
        var registerResponse = await client.PostAsJsonAsync("/auth/register", registerRequest);
        var initialTokens = await registerResponse.Content.ReadFromJsonAsync<TokenResponse>();
        Assert.NotNull(initialTokens);

        var refreshRequest = new RefreshTokenRequest(initialTokens!.RefreshToken);
        var response = await client.PostAsJsonAsync("/auth/refresh", refreshRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var newTokens = await response.Content.ReadFromJsonAsync<TokenResponse>();
        Assert.NotNull(newTokens);
        Assert.NotEqual(initialTokens.AccessToken, newTokens!.AccessToken);
        Assert.NotEqual(initialTokens.RefreshToken, newTokens.RefreshToken);
    }

    [Fact]
    public async Task GetCurrentUser_With_Valid_Token_Returns_Current_User()
    {
        var client = _factory.CreateClient();
        var registerRequest = new RegisterRequest("currentuser", "current@example.com", "Password123!");
        var registerResponse = await client.PostAsJsonAsync("/auth/register", registerRequest);
        var tokens = await registerResponse.Content.ReadFromJsonAsync<TokenResponse>();
        Assert.NotNull(tokens);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens!.AccessToken);
        var response = await client.GetAsync("/auth/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var currentUser = await response.Content.ReadFromJsonAsync<CurrentUser>();
        Assert.NotNull(currentUser);
        Assert.True(currentUser!.IsAuthenticated);
        Assert.Equal("currentuser", currentUser.UserName);
        Assert.Equal("current@example.com", currentUser.Email);
        Assert.NotEqual(Guid.Empty, currentUser.UserId);
    }

    [Fact]
    public async Task GetCurrentUser_Without_Token_Returns_Unauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/auth/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
