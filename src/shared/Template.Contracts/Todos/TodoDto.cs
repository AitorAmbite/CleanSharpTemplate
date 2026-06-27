namespace Template.Contracts.Todos;

public record TodoDto(
    Guid Id,
    string Title,
    bool IsCompleted,
    DateTime CreatedAt);
