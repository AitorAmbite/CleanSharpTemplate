namespace Template.Application.Tests.Events;

using Microsoft.Extensions.Logging;
using Template.Application.Features.Todos.Events;
using Template.Domain.Events;

public class TodoCreatedHandlerTests
{
    [Fact]
    public void Handle_Logs_Todo_Id_And_Title()
    {
        var logger = new SpyLogger<TodoCreatedHandler>();
        var handler = new TodoCreatedHandler(logger);
        var @event = new TodoCreated(Guid.CreateVersion7(), "Buy milk");

        handler.Handle(@event);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Information, entry.Level);
        Assert.Contains(@event.TodoId.ToString(), entry.Message);
        Assert.Contains(@event.Title, entry.Message);
    }

    private sealed class SpyLogger<TCategoryName> : ILogger<TCategoryName>
    {
        public List<(LogLevel Level, string Message)> Entries { get; } = [];

        public IDisposable BeginScope<TState>(TState state)
            where TState : notnull
            => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
            => Entries.Add((logLevel, formatter(state, exception)));

        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();

            public void Dispose()
            {
            }
        }
    }
}