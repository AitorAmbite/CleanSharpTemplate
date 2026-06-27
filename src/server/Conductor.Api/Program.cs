using Conductor.Api.Features.Todos;
using Conductor.Api.Health;
using Conductor.Api.Middleware;
using Conductor.Application;
using Conductor.Infrastructure;
using Conductor.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Host.AddApplication(opts =>
{
    // EF Core registers DbContextOptions<T> as an opaque lambda factory, so Wolverine
    // cannot generate a constructor for handlers that depend on the DbContext directly
    // or indirectly. We opt the DbContext itself into service location only.
    opts.CodeGeneration.AlwaysUseServiceLocationFor<ConductorDbContext>();
});

builder.Services.AddOpenApi();
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database");

builder.Services.AddCors(options =>
{
    options.AddPolicy("Default", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Apply pending migrations with retry for container startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ConductorDbContext>();
    await dbContext.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
app.UseExceptionHandling();
app.UseCors("Default");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapHealthChecks("/health");
app.MapTodoEndpoints();

app.Run();
