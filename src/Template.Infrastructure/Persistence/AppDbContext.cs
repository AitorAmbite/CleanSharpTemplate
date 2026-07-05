namespace Template.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Template.Domain;
using Template.Domain.Entities;

public class AppDbContext : DbContext, IUnitOfWork
{
    public DbSet<Todo> Todos => Set<Todo>();

    public DbSet<User> Users => Set<User>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
