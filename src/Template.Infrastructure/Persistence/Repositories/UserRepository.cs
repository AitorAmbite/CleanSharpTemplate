namespace Template.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Template.Domain.Entities;
using Template.Domain.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return DbSet
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return DbSet
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return DbSet.AnyAsync(u => u.Email == email, cancellationToken);
    }

    public Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return DbSet.AnyAsync(u => u.Username == username, cancellationToken);
    }
}
