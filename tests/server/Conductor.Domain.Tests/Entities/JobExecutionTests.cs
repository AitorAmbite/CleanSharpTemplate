namespace Conductor.Domain.Tests.Entities;

using Conductor.Domain;
using Conductor.Domain.Enums;

public class JobExecutionTests
{
    [Fact]
    public void Constructor_InitializesDefaults()
    {
        var execution = new JobExecution();

        Assert.Equal(default, execution.ScheduledTime);
        Assert.Null(execution.StartTime);
        Assert.Null(execution.EndTime);
        Assert.Null(execution.Result);
        Assert.Equal(default, execution.Status);
        Assert.Null(execution.ExecutedByAgentId);
    }

    [Fact]
    public void SetResult_SetsValue()
    {
        var execution = new JobExecution();

        execution.SetResult(0, "output", "error", null);

        Assert.NotNull(execution.Result);
        Assert.Equal(0, execution.Result.ExitCode.Value);
        Assert.Equal("output", execution.Result.StdOut);
        Assert.Equal("error", execution.Result.StdErr);
        Assert.Null(execution.Result.ErrorMessage);
    }

    [Fact]
    public void SetResult_WithErrorMessage_SetsValue()
    {
        var execution = new JobExecution();

        execution.SetResult(1, null, null, "task failed");

        Assert.NotNull(execution.Result);
        Assert.Equal(1, execution.Result.ExitCode.Value);
        Assert.Null(execution.Result.StdOut);
        Assert.Null(execution.Result.StdErr);
        Assert.Equal("task failed", execution.Result.ErrorMessage);
    }

    [Fact]
    public void Status_CanBeSet()
    {
        var execution = new JobExecution();

        execution.Status = ExecutionStatus.Failed;

        Assert.Equal(ExecutionStatus.Failed, execution.Status);
    }
}
