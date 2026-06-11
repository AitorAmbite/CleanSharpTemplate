namespace Conductor.Infrastructure.Persistence;

using Conductor.Domain;
using Microsoft.EntityFrameworkCore;

public class ConductorDbContext : DbContext, IUnitOfWork
{
    public ConductorDbContext(DbContextOptions<ConductorDbContext> options)
        : base(options)
    {
    }

    // Example: Add DbSet properties for each aggregate root
    // public DbSet<ExampleEntity> ExampleEntities => Set<ExampleEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ConductorDbContext).Assembly);
    }
}
