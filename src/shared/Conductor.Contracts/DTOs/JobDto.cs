namespace Conductor.Contracts.DTOs;

public class JobDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string CronExpression { get; set; } = null!;
    public bool IsEnabled { get; set; }
    public string? Tags { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    #region "Execution config"
    public string Command { get; set; } = null!;
    public string? Arguments { get; set; }
    public string WorkingDirectory { get; set; } = null!;
    public int? TimeoutSeconds { get; set; }
    #endregion
}
