namespace Template.Application.Tests.Features.Todos;

using Template.Application.Features.Todos.Queries.GetTodos;
using Template.Application.Tests.Fakes;
using Template.Contracts.Todos;
using Template.Domain.Entities;

public class GetTodosQueryHandlerTests
{
    [Fact]
    public async Task Handle_Returns_Paginated_TodoDtos()
    {
        var repository = new FakeTodoRepository();
        await repository.AddAsync(new Todo("First"), CancellationToken.None);
        await repository.AddAsync(new Todo("Second"), CancellationToken.None);

        var handler = new GetTodosQueryHandler(repository);
        var query = new GetTodosQuery(1, 10);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
        Assert.Contains(result.Items, dto => dto.Title == "First");
        Assert.Contains(result.Items, dto => dto.Title == "Second");
    }
}
