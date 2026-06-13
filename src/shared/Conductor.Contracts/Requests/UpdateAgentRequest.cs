namespace Conductor.Contracts.Requests;

public class UpdateAgentRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}
