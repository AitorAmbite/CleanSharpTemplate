namespace Template.Application.Tests.Fakes;

public class FakeTimeProvider : TimeProvider
{
    private DateTimeOffset _now;

    public FakeTimeProvider(DateTimeOffset? now = null)
    {
        _now = now ?? DateTimeOffset.UtcNow;
    }

    public override DateTimeOffset GetUtcNow() => _now;

    public void SetUtcNow(DateTimeOffset now)
    {
        _now = now;
    }

    public void Advance(TimeSpan duration)
    {
        _now = _now.Add(duration);
    }
}
