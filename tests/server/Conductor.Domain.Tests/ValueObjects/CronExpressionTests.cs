namespace Conductor.Domain.Tests.ValueObjects;

using Conductor.Domain.ValueObjects;

public class CronExpressionTests
{
    [Fact]
    public void Create_WithValidExpression_ReturnsInstance()
    {
        var expression = CronExpression.Create("0 9 * * *");

        Assert.NotNull(expression);
        Assert.Equal("0 9 * * *", expression.Value);
    }

    [Theory]
    [InlineData("* * * * *")]
    [InlineData("0 0 1 1 *")]
    [InlineData("*/5 * * * *")]
    public void Create_WithVariousExpressions_ReturnsInstance(string cron)
    {
        var expression = CronExpression.Create(cron);

        Assert.NotNull(expression);
        Assert.Equal(cron, expression.Value);
    }
}
