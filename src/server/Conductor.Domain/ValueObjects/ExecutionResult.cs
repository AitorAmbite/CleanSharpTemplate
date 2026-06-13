namespace Conductor.Domain.ValueObjects;

public class ExecutionResult
{
    public ExitCode ExitCode { get; private set; } = null!;
    public string? StdOut { get; private set; }
    public string? StdErr { get; private set; }
    public string? ErrorMessage { get; private set; }

    private ExecutionResult() { }

    private ExecutionResult(ExitCode exitCode, string? stdOut, string? stdErr, string? errorMessage)
    {
        ExitCode = exitCode;
        StdOut = stdOut;
        StdErr = stdErr;
        ErrorMessage = errorMessage;
    }

    public static ExecutionResult Create(int exitCode, string? stdOut, string? stdErr, string? errorMessage)
    {
        return new ExecutionResult(ExitCode.Create(exitCode), stdOut, stdErr, errorMessage);
    }
}
