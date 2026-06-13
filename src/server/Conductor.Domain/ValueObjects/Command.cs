namespace Conductor.Domain.ValueObjects;

public class Command
{
    public string Value { get; private set; } = null!;
    public string? Arguments { get; private set; }

    private Command() { }

    private Command(string value, string? arguments)
    {
        Value = value;
        Arguments = arguments;
    }

    public static Command Create(string value, string? arguments)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Command cannot be empty.", nameof(value));
        }

        return new Command(value, arguments);
    }
}
