using Conductor.Domain.Common;

namespace Conductor.Domain;

public class Agent : AggregateRoot
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}
