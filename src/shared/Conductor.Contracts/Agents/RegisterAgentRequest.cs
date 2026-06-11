namespace Conductor.Contracts.Agents;

public record RegisterAgentRequest(
    string Hostname,
    string Os,
    string AgentVersion,
    string Ip
);
