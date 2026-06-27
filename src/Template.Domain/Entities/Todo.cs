namespace Template.Domain.Entities;

using Template.Domain.Common;

public class Todo : Entity
{
    public string Title { get; private set; } = string.Empty;
    public bool IsCompleted { get; private set; }

    private Todo()
    {
    }

    public Todo(string title)
    {
        Title = title;
    }

    public void Complete()
    {
        IsCompleted = true;
    }
}
