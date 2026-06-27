namespace Conductor.Infrastructure.Persistence;

using Conductor.Domain;

public class EfUnitOfWork : IUnitOfWork
{
    private readonly ConductorDbContext _dbContext;

    public EfUnitOfWork(ConductorDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _dbContext.SaveChangesAsync(cancellationToken);
}
