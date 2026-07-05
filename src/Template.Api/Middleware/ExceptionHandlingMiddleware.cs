namespace Template.Api.Middleware;

using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _hostEnvironment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment hostEnvironment)
    {
        _next = next;
        _logger = logger;
        _hostEnvironment = hostEnvironment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred while processing request {Path}", context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        switch (exception)
        {
            case ValidationException validationException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return WriteValidationProblemAsync(context, validationException);

            case ArgumentException:
            case InvalidOperationException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return WriteProblemAsync(context, "Bad Request", exception.Message);

            case KeyNotFoundException:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return WriteProblemAsync(context, "Not Found", exception.Message);

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                string detail = _hostEnvironment.IsDevelopment()
                    ? exception.Message
                    : "An unexpected error occurred.";
                return WriteProblemAsync(context, "Internal Server Error", detail);
        }
    }

    private static Task WriteValidationProblemAsync(HttpContext context, ValidationException exception)
    {
        var problemDetails = new ValidationProblemDetails
        {
            Title = "Validation Failed",
            Status = context.Response.StatusCode,
        };

        foreach (FluentValidation.Results.ValidationFailure failure in exception.Errors)
        {
            string key = failure.PropertyName;
            if (problemDetails.Errors.TryGetValue(key, out string[]? existing) && existing is not null)
            {
                problemDetails.Errors[key] = [.. existing, failure.ErrorMessage];
            }
            else
            {
                problemDetails.Errors[key] = [failure.ErrorMessage];
            }
        }

        return context.Response.WriteAsJsonAsync(problemDetails, (JsonSerializerOptions?)null, "application/problem+json", context.RequestAborted);
    }

    private static Task WriteProblemAsync(HttpContext context, string title, string detail)
    {
        var problemDetails = new ProblemDetails
        {
            Title = title,
            Status = context.Response.StatusCode,
            Detail = detail,
            Instance = context.Request.Path,
        };

        return context.Response.WriteAsJsonAsync(problemDetails, (JsonSerializerOptions?)null, "application/problem+json", context.RequestAborted);
    }
}