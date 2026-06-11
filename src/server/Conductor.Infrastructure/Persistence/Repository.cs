namespace Conductor.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Conductor.Domain.Common;
using Conductor.Domain.Repositories;

public abstract class Repository<T> : IRepository<T>
    where T : AggregateRoot
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

    public virtual Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => DbSet.ToListAsync(cancellationToken).ContinueWith(t => (IReadOnlyList<T>)t.Result);

    public virtual Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => DbSet.AddAsync(entity, cancellationToken).AsTask();

    public virtual void Update(T entity)
        => DbSet.Update(entity);

    public virtual void Remove(T entity)
        => DbSet.Remove(entity);
}
