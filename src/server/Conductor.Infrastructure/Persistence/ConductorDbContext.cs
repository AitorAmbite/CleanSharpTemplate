namespace Conductor.Infrastructure.Persistence;

using Conductor.Domain;
using Microsoft.EntityFrameworkCore;

public class ConductorDbContext : DbContext, IUnitOfWork
{
    public ConductorDbContext(DbContextOptions<ConductorDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ConductorDbContext).Assembly);
    }
}
