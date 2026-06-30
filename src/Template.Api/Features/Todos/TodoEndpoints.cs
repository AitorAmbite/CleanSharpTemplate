namespace Template.Api.Features.Todos;

using Template.Application.Features.Todos;
using Template.Contracts.Todos;
using Template.Domain.Common;
using Template.Domain.Events;
using Wolverine;

public static class TodoEndpoints
{
    public static IEndpointRouteBuilder MapTodoEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/todos", Get)
        .WithName("GetTodos");

        app.MapPost("/todos", Create)
        .WithName("CreateTodo");

        return app;
    }
    private static async Task<IResult> Create(CreateTodoCommand command,
            IMessageBus bus,
            CancellationToken cancellationToken = default)
    {
        var @event = await bus.InvokeAsync<TodoCreated>(command, cancellationToken);
        return Results.Created($"/todos/{@event.TodoId}", @event);
    }

    private static async Task<PaginatedList<TodoDto>> Get(
        IMessageBus bus,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTodosQuery(page, pageSize);
        return await bus.InvokeAsync<PaginatedList<TodoDto>>(query, cancellationToken);
    }
}
