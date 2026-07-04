namespace Template.Domain.Common;

public abstract class Entity
{
    public Guid Id { get; protected set; } = Guid.CreateVersion7();

    public DateTime CreatedAt { get; internal set; }

    public DateTime? UpdatedAt { get; internal set; }
}