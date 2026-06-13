namespace Conductor.Domain.Tests.Entities;

using Conductor.Domain;

public class JobTests
{
    [Fact]
    public void Constructor_InitializesDefaults()
    {
        var job = new Job();

        Assert.True(job.IsEnabled);
        Assert.False(job.IsDeleted);
        Assert.Empty(job.Executions);
    }

    [Fact]
    public void SetCronExpression_SetsValue()
    {
        var job = new Job();

        job.SetCronExpression("0 9 * * *");

        Assert.NotNull(job.CronExpression);
        Assert.Equal("0 9 * * *", job.CronExpression.Value);
    }

    [Fact]
    public void SetCommand_SetsValue()
    {
        var job = new Job();

        job.SetCommand("echo", "hello", "/tmp", 30);

        Assert.NotNull(job.Command);
        Assert.Equal("echo", job.Command.Value);
        Assert.Equal("hello", job.Command.Arguments);
        Assert.Equal("/tmp", job.Command.WorkingDirectory);
        Assert.Equal(30, job.Command.TimeoutSeconds);
    }

    [Fact]
    public void SetCommand_WithEmptyCommand_ThrowsArgumentException()
    {
        var job = new Job();

        Assert.Throws<ArgumentException>(() =>
            job.SetCommand("", "args", "/tmp", 10));
    }

    [Fact]
    public void SetCommand_WithEmptyWorkingDirectory_ThrowsArgumentException()
    {
        var job = new Job();

        Assert.Throws<ArgumentException>(() =>
            job.SetCommand("echo", "args", "", 10));
    }

    [Fact]
    public void SetCommand_WithNullArguments_SetsValue()
    {
        var job = new Job();

        job.SetCommand("ls", null, "/home", null);

        Assert.NotNull(job.Command);
        Assert.Null(job.Command.Arguments);
        Assert.Null(job.Command.TimeoutSeconds);
    }
}
