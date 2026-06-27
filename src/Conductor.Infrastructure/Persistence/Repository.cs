namespace Conductor.Infrastructure.Persistence;

using Mapster;
using Microsoft.EntityFrameworkCore;
using Conductor.Domain.Common;
using Conductor.Domain.Repositories;

public abstract class Repository<T> : IRepository<T>
    where T : Entity
{
    protected readonly ConductorDbContext DbContext;
    protected readonly DbSet<T> DbSet;

    protected Repository(ConductorDbContext dbContext)
    {
        DbContext = dbContext;
        DbSet = dbContext.Set<T>();
    }

    public virtual Task<T?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => DbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await DbSet.ToListAsync(cancellationToken);

    public virtual async Task<PaginatedList<T>> GetPaginatedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var totalCount = await DbSet.CountAsync(cancellationToken);
        var items = await DbSet
            .OrderByDescending(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<T>(items, totalCount, page, pageSize);
    }

    public virtual async Task<PaginatedList<TResult>> GetPaginatedAsync<TResult>(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var totalCount = await DbSet.CountAsync(cancellationToken);
        var items = await DbSet
            .OrderByDescending(e => e.CreatedAt)
            .ProjectToType<TResult>()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<TResult>(items, totalCount, page, pageSize);
    }

    public virtual Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => DbSet.AddAsync(entity, cancellationToken).AsTask();

    public virtual void Update(T entity)
        => DbSet.Update(entity);

    public virtual void Remove(T entity)
        => DbSet.Remove(entity);
}
