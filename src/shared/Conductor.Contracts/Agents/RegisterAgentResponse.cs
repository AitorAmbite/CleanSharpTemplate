namespace Conductor.Contracts.Agents;

public record RegisterAgentResponse(
    Guid AgentId,
    string Jwt,
    DateTime ExpiresAt
);
