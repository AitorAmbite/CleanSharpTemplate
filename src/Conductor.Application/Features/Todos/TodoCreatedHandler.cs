namespace Conductor.Application.Features.Todos;

using Conductor.Domain.Events;
using Microsoft.Extensions.Logging;

public class TodoCreatedHandler
{
    private readonly ILogger<TodoCreatedHandler> _logger;

    public TodoCreatedHandler(ILogger<TodoCreatedHandler> logger)
    {
        _logger = logger;
    }

    public void Handle(TodoCreated @event)
    {
        _logger.LogInformation("Todo created: {TodoId} - {Title}", @event.TodoId, @event.Title);
    }
}
