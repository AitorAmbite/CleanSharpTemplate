using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Conductor.Application;

public static class QueryableExtensions
{
    public static async Task<PaginatedList<TDestination>> ToPaginatedListAsync<TDestination>(
        this IQueryable query,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var projected = query.ProjectToType<TDestination>();
        return await PaginatedList<TDestination>.CreateAsync(
            projected, page, pageSize, cancellationToken);
    }
}
