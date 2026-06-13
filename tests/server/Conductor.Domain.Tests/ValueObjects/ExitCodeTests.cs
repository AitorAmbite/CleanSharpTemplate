namespace Conductor.Domain.Tests.ValueObjects;

using Conductor.Domain.ValueObjects;

public class ExitCodeTests
{
    [Fact]
    public void Create_WithZero_ReturnsInstance()
    {
        var exitCode = ExitCode.Create(0);

        Assert.NotNull(exitCode);
        Assert.Equal(0, exitCode.Value);
    }

    [Fact]
    public void Create_WithNonZero_ReturnsInstance()
    {
        var exitCode = ExitCode.Create(127);

        Assert.NotNull(exitCode);
        Assert.Equal(127, exitCode.Value);
    }

    [Fact]
    public void Create_WithNegativeValue_ReturnsInstance()
    {
        var exitCode = ExitCode.Create(-1);

        Assert.NotNull(exitCode);
        Assert.Equal(-1, exitCode.Value);
    }
}
