namespace Conductor.Infrastructure.Persistence;

using Conductor.Domain;
using Microsoft.EntityFrameworkCore;

public class ConductorDbContext : DbContext, IUnitOfWork
{
    public ConductorDbContext(DbContextOptions<ConductorDbContext> options)
        : base(options)
    {
    }

    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<JobExecution> JobExecutions => Set<JobExecution>();
    public DbSet<Agent> Agents => Set<Agent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ConductorDbContext).Assembly);
    }
}
