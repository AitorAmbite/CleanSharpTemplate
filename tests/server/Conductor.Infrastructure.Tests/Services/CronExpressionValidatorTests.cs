namespace Conductor.Infrastructure.Tests.Services;

using Conductor.Infrastructure.Services;

public class CronExpressionValidatorTests
{
    private readonly CronExpressionValidator _validator = new();

    [Theory]
    [InlineData("* * * * *")]
    [InlineData("0 9 * * *")]
    [InlineData("*/5 * * * *")]
    [InlineData("0 0 1 1 *")]
    [InlineData("0 0 * * 0")]
    [InlineData("0 0 L * *")]
    public void Valid_WithValidCronExpression_ReturnsTrue(string expression)
    {
        var result = _validator.Valid(expression);

        Assert.True(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("invalid")]
    [InlineData("1 2 3")]
    [InlineData("a b c d e")]
    [InlineData("99 99 99 99 99")]
    public void Valid_WithInvalidCronExpression_ReturnsFalse(string expression)
    {
        var result = _validator.Valid(expression);

        Assert.False(result);
    }
}
