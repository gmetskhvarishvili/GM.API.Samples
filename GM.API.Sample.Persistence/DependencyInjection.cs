using GM.API.Sample.Domain.BoundedContext.SampleBoundedContext.SampleAggregate;
using GM.API.Sample.Domain.SeedWork;
using GM.API.Sample.Persistence.Context;
using GM.API.Sample.Persistence.Repositories;
using GM.EntityFramework.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GM.API.Sample.Persistence;

public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            // Pooled DbContext using the framework's own service provider. Avoid
            // AddEntityFrameworkNpgsql()/UseInternalServiceProvider — that opts out of EF Core's
            // shared service caching and is discouraged unless you inject custom EF services.
            services.AddDbContextPool<ApplicationDbContext>((serviceProvider, options) =>
            {
                options.UseNpgsql(configuration.GetConnectionString("ApplicationDatabase"),
                    o =>
                    {
                        o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                        o.CommandTimeout(60);
                    });
                options.AddGMActorAuditing(serviceProvider);
            });

            services.AddTransient<ISampleRepository, SampleRepository>();
            services.AddTransient<IUnitOfWork, UnitOfWork.UnitOfWork>();

            return services;
        }
    }