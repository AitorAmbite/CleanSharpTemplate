namespace Conductor.Api.Features.Todos;

using Conductor.Application.Features.Todos;
using Conductor.Contracts.Todos;
using Conductor.Domain.Common;
using Conductor.Domain.Events;
using Wolverine;

public static class TodoEndpoints
{
    public static IEndpointRouteBuilder MapTodoEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/todos", async (
            IMessageBus bus,
            int page = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default) =>
        {
            var query = new GetTodosQuery(page, pageSize);
            return await bus.InvokeAsync<PaginatedList<TodoDto>>(query, cancellationToken);
        })
        .WithName("GetTodos");

        app.MapPost("/todos", async (
            CreateTodoCommand command,
            IMessageBus bus,
            CancellationToken cancellationToken = default) =>
        {
            var @event = await bus.InvokeAsync<TodoCreated>(command, cancellationToken);
            return Results.Created($"/todos/{@event.TodoId}", @event);
        })
        .WithName("CreateTodo");

        return app;
    }
}
