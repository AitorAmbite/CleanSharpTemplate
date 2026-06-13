using Conductor.Domain.Common;
using Conductor.Domain.Enums;
using Conductor.Domain.ValueObjects;

namespace Conductor.Domain;

public class JobExecution : AggregateRoot
{
    public Guid JobId { get; set; }
    public virtual Job Job { get; set; } = null!;

    public DateTime ScheduledTime { get; set; }   // momento en que debía ejecutarse (cron)
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    public ExitCode? ExitCode { get; private set; }
    public string? StdOut { get; set; }
    public string? StdErr { get; set; }
    public ExecutionStatus Status { get; set; }
    public string? ErrorMessage { get; set; }

    // Agente que ejecutó (null = servidor central)
    public Guid? ExecutedByAgentId { get; set; }
    public virtual Agent? ExecutedByAgent { get; set; }

    public void SetExitCode(int value)
    {
        ExitCode = ValueObjects.ExitCode.Create(value);
    }
}