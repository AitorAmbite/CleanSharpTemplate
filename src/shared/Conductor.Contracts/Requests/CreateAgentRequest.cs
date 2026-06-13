namespace Conductor.Contracts.Requests;

public class CreateAgentRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
