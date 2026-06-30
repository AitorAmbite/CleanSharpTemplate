namespace Template.Integration.Tests.Features.Todos;

using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Template.Contracts.Todos;
using Template.Domain.Common;
using Template.Domain.Events;
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
            .WithDatabase("conductor")
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
        var dbContext = scope.ServiceProvider.GetRequiredService<ConductorDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public Task DisposeAsync()
    {
        _factory.Dispose();
        return _postgres.DisposeAsync().AsTask();
    }

    [Fact]
    public async Task CreateTodo_Returns_Created_With_TodoCreated_Event()
    {
        var client = _factory.CreateClient();
        var command = new { Title = "Integration test todo" };

        var response = await client.PostAsJsonAsync("/todos", command);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var @event = await response.Content.ReadFromJsonAsync<TodoCreated>();
        Assert.NotNull(@event);
        Assert.Equal("Integration test todo", @event.Title);
        Assert.NotEqual(Guid.Empty, @event.TodoId);
    }

    [Fact]
    public async Task GetTodos_Returns_Paginated_List_Of_TodoDtos()
    {
        var client = _factory.CreateClient();
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
}
