namespace Conductor.Contracts.Requests;

public class UpdateJobRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string CronExpression { get; set; } = null!;
    public string Command { get; set; } = null!;
    public string? Arguments { get; set; }
    public string WorkingDirectory { get; set; } = null!;
    public int? TimeoutSeconds { get; set; }
    public string? Tags { get; set; }
    public bool IsEnabled { get; set; }
}
