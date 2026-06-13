namespace Conductor.Domain.Tests.Entities;

using Conductor.Domain;

public class AgentTests
{
    [Fact]
    public void Constructor_InitializesDefaults()
    {
        var agent = new Agent();

        Assert.True(agent.IsActive);
    }

    [Fact]
    public void Name_CanBeSet()
    {
        var agent = new Agent();

        agent.Name = "TestAgent";

        Assert.Equal("TestAgent", agent.Name);
    }

    [Fact]
    public void Description_CanBeSet()
    {
        var agent = new Agent();

        agent.Description = "A test agent";

        Assert.Equal("A test agent", agent.Description);
    }
}
