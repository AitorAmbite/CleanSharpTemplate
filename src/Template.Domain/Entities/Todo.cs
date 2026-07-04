namespace Template.Domain.Entities;

using Template.Domain.Common;

public class Todo : Entity
{
    private const int MaxTitleLength = 200;

    public string Title { get; private set; } = string.Empty;
    public bool IsCompleted { get; private set; }

    private Todo()
    {
    }

    public Todo(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.", nameof(title));
        }

        string trimmed = title.Trim();
        if (trimmed.Length > MaxTitleLength)
        {
            throw new ArgumentException($"Title must be at most {MaxTitleLength} characters.", nameof(title));
        }

        Title = trimmed;
    }

    public void Complete()
    {
        if (IsCompleted)
        {
            throw new InvalidOperationException("Todo is already completed.");
        }

        IsCompleted = true;
    }
}