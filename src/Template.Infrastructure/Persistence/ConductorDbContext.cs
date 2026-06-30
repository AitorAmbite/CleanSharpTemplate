namespace Template.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Template.Domain;
using Template.Domain.Entities;

public class ConductorDbContext : DbContext, IUnitOfWork
{
    public DbSet<Todo> Todos => Set<Todo>();

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
