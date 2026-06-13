using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Conductor.Infrastructure.Persistence;

public class ConductorDbContextFactory : IDesignTimeDbContextFactory<ConductorDbContext>
{
    public ConductorDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ConductorDbContext>();
        optionsBuilder.UseSqlite("Data Source=:memory:");

        return new ConductorDbContext(optionsBuilder.Options);
    }
}
