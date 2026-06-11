namespace Conductor.Contracts.Agents;

public record AgentHeartbeatResponse(
    int? PollingIntervalSec = null
);
