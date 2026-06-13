namespace Conductor.Domain.ValueObjects;

public class Command
{
    public string Value { get; private set; } = null!;
    public string? Arguments { get; private set; }
    public string WorkingDirectory { get; private set; } = null!;
    public int? TimeoutSeconds { get; private set; }

    private Command() { }

    private Command(string value, string? arguments, string workingDirectory, int? timeoutSeconds)
    {
        Value = value;
        Arguments = arguments;
        WorkingDirectory = workingDirectory;
        TimeoutSeconds = timeoutSeconds;
    }

    public static Command Create(string value, string? arguments, string workingDirectory, int? timeoutSeconds)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Command cannot be empty.", nameof(value));
        }

        if (string.IsNullOrWhiteSpace(workingDirectory))
        {
            throw new ArgumentException("Working directory cannot be empty.", nameof(workingDirectory));
        }

        return new Command(value, arguments, workingDirectory, timeoutSeconds);
    }
}
