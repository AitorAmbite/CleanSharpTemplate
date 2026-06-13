using Conductor.Application;

namespace Conductor.Api.Endpoints;

public static class JobExecutionEndpoints
{
    public static IEndpointRouteBuilder MapJobExecutionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/jobexecutions")
            .WithTags("JobExecutions");

        group.MapGet("/job/{jobId:guid}", GetByJob);
        group.MapGet("/{id:guid}", GetById);

        return app;
    }

    private static Task<IResult> GetByJob(Guid jobId, CancellationToken cancellationToken)
    {
        var response = PaginatedList<object>.Create(Array.Empty<object>(), 0, 1, 10);
        return Task.FromResult(Results.Ok(response));
    }

    private static Task<IResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(Results.NotFound());
    }
}
