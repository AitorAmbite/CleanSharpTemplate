using Conductor.Domain.Common;
using Conductor.Domain.ValueObjects;

namespace Conductor.Domain;

public class Job : AggregateRoot
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public CronExpression CronExpression { get; private set; } = null!;
    public bool IsEnabled { get; set; } = true;

    #region "Execution config"
    public Command Command { get; private set; } = null!;
    public string? WorkingDirectory { get; set; }
    public int? TimeoutSeconds { get; set; }

    #endregion

    public string? Tags { get; set; }
    public bool IsDeleted { get; set; }

    #region  "Agent config"
    // Future versions
    // public Guid? AssignedAgentId { get; set; }
    // public virtual Agent? AssignedAgent { get; set; }
    #endregion
    public virtual ICollection<JobExecution> Executions { get; set; } = new List<JobExecution>();

    public void SetCronExpression(string cronExpression)
    {
        CronExpression = ValueObjects.CronExpression.Create(cronExpression);
    }

    public void SetCommand(string command, string? arguments)
    {
        Command = ValueObjects.Command.Create(command, arguments);
    }
}