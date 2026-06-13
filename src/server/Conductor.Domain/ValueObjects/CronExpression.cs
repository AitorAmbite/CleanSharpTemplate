namespace Conductor.Domain.ValueObjects;

public class CronExpression
{
    public string Value { get; private set; } = null!;

    private CronExpression() { }

    private CronExpression(string value)
    {
        Value = value;
    }

    public static CronExpression Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Cron expression cannot be empty.", nameof(value));
        }

        var parts = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 5 || parts.Length > 6)
        {
            throw new ArgumentException("Invalid cron expression format. Expected 5 or 6 fields.", nameof(value));
        }

        return new CronExpression(value);
    }
}
