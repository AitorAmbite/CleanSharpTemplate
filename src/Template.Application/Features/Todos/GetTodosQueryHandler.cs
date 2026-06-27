namespace Template.Application.Features.Todos;

using Template.Contracts.Todos;
using Template.Domain.Common;
using Template.Domain.Repositories;

public class GetTodosQueryHandler
{
    private readonly ITodoRepository _todoRepository;

    public GetTodosQueryHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public Task<PaginatedList<TodoDto>> Handle(GetTodosQuery query, CancellationToken cancellationToken)
    {
        return _todoRepository.GetPaginatedAsync<TodoDto>(query.Page, query.PageSize, cancellationToken);
    }
}
