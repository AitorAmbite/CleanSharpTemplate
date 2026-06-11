using Conductor.Contracts.Enums;

namespace Conductor.Contracts.Agents;

public record AgentHeartbeatRequest(
    double CpuPercent,
    long MemoryMb,
    AgentStatus Status,
    string AgentVersion
);
