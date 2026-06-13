namespace Conductor.Domain.ValueObjects;

public class CronExpression
{
    public string Value { get; private set; } = null!;

    private CronExpression(string value)
    {
        Value = value;
    }

    public static CronExpression Create(string value)
    {
        return new CronExpression(value);
    }
}
