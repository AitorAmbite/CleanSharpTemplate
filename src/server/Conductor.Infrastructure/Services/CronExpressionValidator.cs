namespace Conductor.Infrastructure.Services;

using Conductor.Domain;
using Cronos;

public class CronExpressionValidator : ICronExpressionValidator
{
    public bool Valid(string cronExpression)
    {
        return CronExpression.TryParse(cronExpression, out _);
    }
}
