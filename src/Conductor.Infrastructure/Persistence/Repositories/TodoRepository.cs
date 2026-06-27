namespace Conductor.Infrastructure.Persistence.Repositories;

using Conductor.Domain.Entities;
using Conductor.Domain.Repositories;

public class TodoRepository : Repository<Todo>, ITodoRepository
{
    public TodoRepository(ConductorDbContext dbContext)
        : base(dbContext)
    {
    }
}
