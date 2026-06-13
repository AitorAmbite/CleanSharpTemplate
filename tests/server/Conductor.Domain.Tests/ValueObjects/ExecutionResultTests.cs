namespace Conductor.Domain.Tests.ValueObjects;

using Conductor.Domain.ValueObjects;

public class ExecutionResultTests
{
    [Fact]
    public void Create_WithValidParameters_ReturnsInstance()
    {
        var result = ExecutionResult.Create(0, "stdout", "stderr", null);

        Assert.NotNull(result);
        Assert.NotNull(result.ExitCode);
        Assert.Equal(0, result.ExitCode.Value);
        Assert.Equal("stdout", result.StdOut);
        Assert.Equal("stderr", result.StdErr);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void Create_WithNullOutputs_ReturnsInstance()
    {
        var result = ExecutionResult.Create(1, null, null, "error occurred");

        Assert.NotNull(result);
        Assert.Equal(1, result.ExitCode.Value);
        Assert.Null(result.StdOut);
        Assert.Null(result.StdErr);
        Assert.Equal("error occurred", result.ErrorMessage);
    }

    [Fact]
    public void Create_WithAllNullOutputs_ReturnsInstance()
    {
        var result = ExecutionResult.Create(0, null, null, null);

        Assert.NotNull(result);
        Assert.Equal(0, result.ExitCode.Value);
        Assert.Null(result.StdOut);
        Assert.Null(result.StdErr);
        Assert.Null(result.ErrorMessage);
    }
}
