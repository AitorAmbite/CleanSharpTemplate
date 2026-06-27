namespace Template.Application.Tests.Features.Todos;

using Template.Application.Features.Todos;
using Template.Application.Tests.Fakes;
using Template.Domain.Events;

public class CreateTodoHandlerTests
{
    [Fact]
    public async Task Handle_Creates_Todo_And_Returns_TodoCreated_Event()
    {
        var repository = new FakeTodoRepository();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new CreateTodoHandler(repository, unitOfWork);
        var command = new CreateTodoCommand("Buy milk");

        TodoCreated result = await handler.Handle(command, CancellationToken.None);

        Assert.Single(repository.AddedEntities);
        Assert.Equal("Buy milk", repository.AddedEntities[0].Title);
        Assert.Equal(result.TodoId, repository.AddedEntities[0].Id);
        Assert.Equal("Buy milk", result.Title);
        Assert.Equal(1, unitOfWork.SaveChangesCount);
    }
}
