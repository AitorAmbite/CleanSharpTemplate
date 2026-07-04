namespace Template.Integration.Tests.Middleware;

using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Template.Api.Middleware;

public class ExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_With_Unknown_Exception_In_Production_Returns_Generic_Message()
    {
        var env = Substitute.For<IHostEnvironment>();
        env.EnvironmentName.Returns("Production");
        var middleware = new ExceptionHandlingMiddleware(
            _ => throw new InvalidOperationException("internal db stack detail"),
            Substitute.For<ILogger<ExceptionHandlingMiddleware>>(),
            env);

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Request.Path = "/todos";

        await middleware.InvokeAsync(context);

        Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);

        context.Response.Body.Position = 0;
        var problem = await JsonSerializer.DeserializeAsync<ProblemDetails>(context.Response.Body!);
        Assert.NotNull(problem);
        Assert.Equal("Internal Server Error", problem!.Title);
        Assert.Equal("An unexpected error occurred.", problem.Detail);
        Assert.Equal("/todos", problem.Instance!.ToString());
    }

    [Fact]
    public async Task InvokeAsync_With_Validation_Exception_Returns_Bad_Request_With_Errors()
    {
        var env = Substitute.For<IHostEnvironment>();
        env.EnvironmentName.Returns("Development");
        var middleware = new ExceptionHandlingMiddleware(
            _ => throw new ArgumentException("Title is required."),
            Substitute.For<ILogger<ExceptionHandlingMiddleware>>(),
            env);

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Request.Path = "/todos";

        await middleware.InvokeAsync(context);

        Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);

        context.Response.Body.Position = 0;
        var problem = await JsonSerializer.DeserializeAsync<ProblemDetails>(context.Response.Body!);
        Assert.NotNull(problem);
        Assert.Equal("Bad Request", problem!.Title);
        Assert.Equal("Title is required.", problem.Detail);
    }
}