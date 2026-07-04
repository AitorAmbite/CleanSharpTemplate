namespace Template.Application.Tests.Features.Todos;

using Template.Application.Features.Todos;
using Template.Application.Tests.Fakes;
using Template.Contracts.Todos;
using Template.Domain.Events;

public class CreateTodoHandlerTests
{
    [Fact]
    public async Task Handle_Creates_Todo_And_Returns_CreateTodoResponse_With_Cascaded_TodoCreated()
    {
        var repository = new FakeTodoRepository();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new CreateTodoHandler(repository, unitOfWork);
        var command = new CreateTodoCommand("Buy milk");

        var (response, @event) = await handler.Handle(command, CancellationToken.None);

        Assert.Single(repository.AddedEntities);
        Assert.Equal("Buy milk", repository.AddedEntities[0].Title);

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal("Buy milk", response.Title);

        Assert.Equal(repository.AddedEntities[0].Id, @event.TodoId);
        Assert.Equal("Buy milk", @event.Title);

        Assert.Equal(1, unitOfWork.SaveChangesCount);
    }
}
