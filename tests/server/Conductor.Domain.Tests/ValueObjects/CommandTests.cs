namespace Conductor.Domain.Tests.ValueObjects;

using Conductor.Domain.ValueObjects;

public class CommandTests
{
    [Fact]
    public void Create_WithValidParameters_ReturnsInstance()
    {
        var command = Command.Create("echo", "hello", "/tmp", 30);

        Assert.NotNull(command);
        Assert.Equal("echo", command.Value);
        Assert.Equal("hello", command.Arguments);
        Assert.Equal("/tmp", command.WorkingDirectory);
        Assert.Equal(30, command.TimeoutSeconds);
    }

    [Fact]
    public void Create_WithNullArguments_ReturnsInstance()
    {
        var command = Command.Create("ls", null, "/home", null);

        Assert.NotNull(command);
        Assert.Null(command.Arguments);
    }

    [Fact]
    public void Create_WithEmptyCommand_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            Command.Create("", "args", "/tmp", 10));

        Assert.Contains("Command", exception.Message);
    }

    [Fact]
    public void Create_WithWhitespaceCommand_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            Command.Create("   ", "args", "/tmp", 10));

        Assert.Contains("Command", exception.Message);
    }

    [Fact]
    public void Create_WithEmptyWorkingDirectory_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            Command.Create("echo", "args", "", 10));

        Assert.Contains("Working directory", exception.Message);
    }

    [Fact]
    public void Create_WithWhitespaceWorkingDirectory_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            Command.Create("echo", "args", "   ", 10));

        Assert.Contains("Working directory", exception.Message);
    }
}
