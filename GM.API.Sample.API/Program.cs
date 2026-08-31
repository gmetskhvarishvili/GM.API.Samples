using GM.API.Sample.Persistence;
using GM.API.Sample.Persistence.Context;
using GM.API.Startup;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;

var builder = ProgramExtension.CreateGMBuilder(args);

builder.Services.ConfigureGMServices(
    builder.Configuration,
    "policyName",
    "SwaggerDocOptions");

builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseGMServices();

// Liveness never depends on downstream dependencies; readiness runs the registered checks.
app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false });
app.MapHealthChecks("/health/ready");

using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
        if (context != null)
        {
            await context.Database.MigrateAsync();

            var logger = scope.ServiceProvider.GetService<ILogger<ApplicationDbContextSeed>>();
            if (logger != null)
                await new ApplicationDbContextSeed().SeedAsync(context, logger);
        }
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or initializing the database.");
    }
}

await app.RunAsync();