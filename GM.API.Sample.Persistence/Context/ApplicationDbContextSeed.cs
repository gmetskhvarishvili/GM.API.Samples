using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace GM.API.Sample.Persistence.Context;

public class ApplicationDbContextSeed
{
    [SuppressMessage("Design", "S108:Nested blocks of code should not be left empty",
        Justification = "Intentionally empty: this sample ships no seed data. Add your own seeding " +
                         "logic here; the surrounding try/catch demonstrates a bounded-retry pattern " +
                         "for transient failures (e.g. the database not being ready yet after migration).")]
    public async Task SeedAsync(ApplicationDbContext context,
        ILogger<ApplicationDbContextSeed> logger, int? retry = 0)
    {
        int retryForAvaiability = retry ?? 0;

        try
        {
            // Add sample seed data here.
        }
        catch (Exception ex)
        {
            if (retryForAvaiability < 10)
            {
                retryForAvaiability++;

                logger.LogError(ex, "EXCEPTION ERROR while migrating {DbContextName}", nameof(ApplicationDbContext));

                await SeedAsync(context, logger, retryForAvaiability);
            }
        }
    }
}