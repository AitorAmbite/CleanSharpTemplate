using Conductor.Domain.Common;
using Conductor.Domain.Enums;
using Conductor.Domain.ValueObjects;

namespace Conductor.Domain;

public class JobExecution : AggregateRoot
{
    public Guid JobId { get; set; }
    public virtual Job Job { get; set; } = null!;

    public DateTime ScheduledTime { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    public ExecutionResult? Result { get; private set; }
    public ExecutionStatus Status { get; set; }

    // Agente que ejecutó (null = servidor central)
    public Guid? ExecutedByAgentId { get; set; }
    public virtual Agent? ExecutedByAgent { get; set; }

    public void SetResult(int exitCode, string? stdOut, string? stdErr, string? errorMessage)
    {
        Result = ExecutionResult.Create(exitCode, stdOut, stdErr, errorMessage);
    }
}
