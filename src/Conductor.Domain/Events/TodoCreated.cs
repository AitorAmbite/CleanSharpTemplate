namespace Conductor.Domain.Events;

public record TodoCreated(Guid TodoId, string Title);
