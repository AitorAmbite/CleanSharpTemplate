namespace Template.Application.Features.Todos;

using Microsoft.Extensions.Logging;
using Template.Domain.Events;

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
