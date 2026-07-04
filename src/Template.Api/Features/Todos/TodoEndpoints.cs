namespace Template.Api.Features.Todos;

using Template.Application.Features.Todos;
using Template.Contracts;
using Template.Contracts.Todos;
using Wolverine;

public static class TodoEndpoints
{
    public static IEndpointRouteBuilder MapTodoEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/todos", GetTodosAsync)
            .WithName("GetTodos");

        app.MapPost("/todos", CreateTodoAsync)
            .WithName("CreateTodo");

        return app;
    }

    private static async Task<IResult> CreateTodoAsync(
        CreateTodoCommand command,
        IMessageBus bus,
        CancellationToken cancellationToken = default)
    {
        var response = await bus.InvokeAsync<CreateTodoResponse>(command, cancellationToken);
        return Results.Created($"/todos/{response.Id}", response);
    }

    private static async Task<PaginatedList<TodoDto>> GetTodosAsync(
        IMessageBus bus,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTodosQuery(page, pageSize);
        return await bus.InvokeAsync<PaginatedList<TodoDto>>(query, cancellationToken);
    }
}
