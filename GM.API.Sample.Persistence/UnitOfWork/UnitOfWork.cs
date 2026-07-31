using GM.API.Sample.Domain.BoundedContext.SampleBoundedContext.SampleAggregate;
using GM.API.Sample.Domain.SeedWork;
using GM.API.Sample.Persistence.Context;
using GM.EntityFramework.Persistence;

namespace GM.API.Sample.Persistence.UnitOfWork;

public class UnitOfWork(
    ApplicationDbContext context, 
    ISampleRepository sampleRepository)
    : GenericUnitOfWork<ApplicationDbContext>(context), IUnitOfWork
{
    public ISampleRepository SampleRepository { get; } = sampleRepository;
}