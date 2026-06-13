namespace Conductor.Domain;

public interface ICronExpressionValidator
{
    bool Valid(string cronExpression);
}
