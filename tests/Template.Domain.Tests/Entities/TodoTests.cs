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
    public void Complete_Sets_IsCompleted_To_True()
    {
        var todo = new Todo("Buy milk");

        todo.Complete();

        Assert.True(todo.IsCompleted);
    }
}
