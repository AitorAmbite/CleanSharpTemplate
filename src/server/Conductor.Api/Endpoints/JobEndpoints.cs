using Conductor.Application;

namespace Conductor.Api.Endpoints;

public static class JobEndpoints
{
    public static IEndpointRouteBuilder MapJobEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/jobs")
            .WithTags("Jobs");

        group.MapGet("/", GetAll);
        group.MapGet("/{id:guid}", GetById);
        group.MapPost("/", Create);
        group.MapPut("/{id:guid}", Update);
        group.MapDelete("/{id:guid}", Delete);

        return app;
    }

    private static Task<IResult> GetAll(CancellationToken cancellationToken)
    {
        var response = PaginatedList<object>.Create(Array.Empty<object>(), 0, 1, 10);
        return Task.FromResult(Results.Ok(response));
    }

    private static Task<IResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(Results.NotFound());
    }

    private static Task<IResult> Create(object request, CancellationToken cancellationToken)
    {
        return Task.FromResult(Results.Created($"/api/jobs/{Guid.NewGuid()}", null));
    }

    private static Task<IResult> Update(Guid id, object request, CancellationToken cancellationToken)
    {
        return Task.FromResult(Results.NoContent());
    }

    private static Task<IResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(Results.NoContent());
    }
}
