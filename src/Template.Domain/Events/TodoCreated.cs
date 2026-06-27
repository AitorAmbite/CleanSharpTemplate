namespace Template.Domain.Events;

public record TodoCreated(Guid TodoId, string Title);
