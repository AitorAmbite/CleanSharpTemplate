namespace Conductor.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

public class ConductorDbContext : DbContext
{
    public ConductorDbContext(DbContextOptions<ConductorDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ConductorDbContext).Assembly);
    }
}
