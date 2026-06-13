namespace Conductor.Domain.ValueObjects;

public class ExitCode
{
    public int Value { get; private set; }

    private ExitCode() { }

    private ExitCode(int value)
    {
        Value = value;
    }

    public static ExitCode Create(int value) => new(value);
}
