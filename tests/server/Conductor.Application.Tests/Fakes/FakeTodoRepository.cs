namespace Conductor.Application.Tests.Fakes;

using Mapster;
using Conductor.Domain.Common;
using Conductor.Domain.Entities;
using Conductor.Domain.Repositories;

public class FakeTodoRepository : ITodoRepository
{
    private readonly List<Todo> _entities = [];

    public IReadOnlyList<Todo> AddedEntities => _entities.ToList().AsReadOnly();

    public Task<Todo?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_entities.FirstOrDefault(e => e.Id == id));

    public Task<IReadOnlyList<Todo>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Todo>>(_entities.ToList());

    public Task<PaginatedList<Todo>> GetPaginatedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var items = _entities
            .OrderByDescending(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult(new PaginatedList<Todo>(items, _entities.Count, page, pageSize));
    }

    public Task<PaginatedList<TResult>> GetPaginatedAsync<TResult>(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var items = _entities
            .OrderByDescending(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsQueryable()
            .ProjectToType<TResult>()
            .ToList();

        return Task.FromResult(new PaginatedList<TResult>(items, _entities.Count, page, pageSize));
    }

    public Task AddAsync(Todo entity, CancellationToken cancellationToken = default)
    {
        _entities.Add(entity);
        return Task.CompletedTask;
    }

    public void Update(Todo entity)
    {
    }

    public void Remove(Todo entity)
    {
        _entities.Remove(entity);
    }
}
