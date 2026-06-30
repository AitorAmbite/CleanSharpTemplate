namespace Template.Infrastructure.Persistence.Interceptors;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Template.Domain.Common;

public class EntityAuditInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateAuditableEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void UpdateAuditableEntities(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        IEnumerable<EntityEntry<Entity>> modifiedEntries = context.ChangeTracker
            .Entries<Entity>()
            .Where(entry => entry.State == EntityState.Modified);

        foreach (EntityEntry<Entity> entry in modifiedEntries)
        {
            entry.Entity.UpdatedAt = DateTime.UtcNow;
        }
    }
}
