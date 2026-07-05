namespace Template.Integration.Tests.Features.Todos;

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Template.Api.Features.Auth.Models;
using Template.Contracts;
using Template.Contracts.Auth;
using Template.Contracts.Todos;
using Template.Infrastructure.Persistence;
using Template.Integration.Tests.Fixtures;
using Testcontainers.PostgreSql;

public class TodoEndpointsTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres;
    private readonly CustomWebApplicationFactory _factory;

    public TodoEndpointsTests()
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
    public async Task CreateTodo_Returns_Created_With_CreateTodoResponse()
    {
        var client = await CreateAuthenticatedClientAsync();
        var command = new { Title = "Integration test todo" };

        var response = await client.PostAsJsonAsync("/todos", command);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<CreateTodoResponse>();
        Assert.NotNull(body);
        Assert.Equal("Integration test todo", body!.Title);
        Assert.NotEqual(Guid.Empty, body.Id);
    }

    [Fact]
    public async Task GetTodos_Returns_Paginated_List_Of_TodoDtos()
    {
        var client = await CreateAuthenticatedClientAsync();
        await client.PostAsJsonAsync("/todos", new { Title = "First" });
        await client.PostAsJsonAsync("/todos", new { Title = "Second" });

        var response = await client.GetAsync("/todos?page=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PaginatedList<TodoDto>>();
        Assert.NotNull(result);
        Assert.True(result.TotalCount >= 2);
        Assert.Contains(result.Items, dto => dto.Title == "First");
        Assert.Contains(result.Items, dto => dto.Title == "Second");
    }

    [Fact]
    public async Task CreateTodo_With_Empty_Title_Returns_BadRequest_With_ValidationProblem()
    {
        var client = await CreateAuthenticatedClientAsync();
        var command = new { Title = "" };

        var response = await client.PostAsJsonAsync("/todos", command);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problem);
        Assert.Contains("Title", problem!.Errors.Keys);
    }

    [Fact]
    public async Task GetTodos_With_Invalid_Pagination_Returns_BadRequest()
    {
        var client = await CreateAuthenticatedClientAsync();

        var response = await client.GetAsync("/todos?page=0&pageSize=200");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetTodos_Without_Token_Returns_Unauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/todos?page=1&pageSize=10");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var client = _factory.CreateClient();

        var registerRequest = new RegisterRequest(
            $"user{Guid.CreateVersion7():N}",
            $"user{Guid.CreateVersion7():N}@example.com",
            "Password123!");

        var response = await client.PostAsJsonAsync("/auth/register", registerRequest);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var tokens = await response.Content.ReadFromJsonAsync<TokenResponse>();
        Assert.NotNull(tokens);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens!.AccessToken);

        return client;
    }
}
