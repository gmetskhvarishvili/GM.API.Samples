using GM.API.Sample.Persistence.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GM.API.Sample.Persistence.Context;

public class ApplicationDbContextFactory : DesignTimeDbContextFactoryBase<ApplicationDbContext>
{
    protected override ApplicationDbContext CreateNewInstance(DbContextOptions<ApplicationDbContext> options)
    {
        return new ApplicationDbContext(options);
    }
}