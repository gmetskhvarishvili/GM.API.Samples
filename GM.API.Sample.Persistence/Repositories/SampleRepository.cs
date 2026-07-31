using GM.API.Sample.Domain.BoundedContext.SampleBoundedContext.SampleAggregate;
using GM.API.Sample.Persistence.Context;
using GM.EntityFramework.Persistence.Repositories;

namespace GM.API.Sample.Persistence.Repositories;

public class SampleRepository(ApplicationDbContext context)
    : GenericRepository<Domain.BoundedContext.SampleBoundedContext.SampleAggregate.Sample,
        ApplicationDbContext>(context), ISampleRepository;