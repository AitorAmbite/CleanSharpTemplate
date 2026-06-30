namespace Template.Infrastructure.Persistence.Repositories;

using Template.Domain.Entities;
using Template.Domain.Repositories;

public class TodoRepository : Repository<Todo>, ITodoRepository
{
    public TodoRepository(AppDbContext dbContext)
        : base(dbContext)
    {
    }
}
