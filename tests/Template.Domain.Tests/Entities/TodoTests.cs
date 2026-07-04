namespace Template.Domain.Tests.Entities;

using Template.Domain.Entities;

public class TodoTests
{
    [Fact]
    public void Constructor_Sets_Title_And_Default_State()
    {
        var todo = new Todo("Buy milk");

        Assert.Equal("Buy milk", todo.Title);
        Assert.False(todo.IsCompleted);
        Assert.NotEqual(Guid.Empty, todo.Id);
    }

    [Fact]
    public void Constructor_Rejects_Empty_Title()
    {
        Assert.Throws<ArgumentException>(() => new Todo(""));
        Assert.Throws<ArgumentException>(() => new Todo("   "));
        Assert.Throws<ArgumentException>(() => new Todo(null!));
    }

    [Fact]
    public void Constructor_Trims_Title()
    {
        var todo = new Todo("  Buy milk  ");

        Assert.Equal("Buy milk", todo.Title);
    }

    [Fact]
    public void Constructor_Rejects_Title_Longer_Than_Limit()
    {
        string tooLong = new string('a', 201);

        Assert.Throws<ArgumentException>(() => new Todo(tooLong));
    }

    [Fact]
    public void Complete_Sets_IsCompleted_To_True()
    {
        var todo = new Todo("Buy milk");

        todo.Complete();

        Assert.True(todo.IsCompleted);
    }

    [Fact]
    public void Complete_Throws_When_Already_Completed()
    {
        var todo = new Todo("Buy milk");
        todo.Complete();

        Assert.Throws<InvalidOperationException>(todo.Complete);
    }
}