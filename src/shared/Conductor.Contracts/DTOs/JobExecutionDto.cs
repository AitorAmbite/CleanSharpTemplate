namespace Conductor.Contracts.DTOs;

using Conductor.Contracts.Enums;

public class JobExecutionDto
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public DateTime ScheduledTime { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? ExitCode { get; set; }
    public string? StdOut { get; set; }
    public string? StdErr { get; set; }
    public ExecutionStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
    public Guid? ExecutedByAgentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
