namespace Conductor.Domain.Common;

public abstract class DomainEvent
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
