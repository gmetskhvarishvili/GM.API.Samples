using GM.API.Sample.Domain.BoundedContext.SampleBoundedContext.SampleAggregate;
using GM.EntityFramework.Domain.Repositories;

namespace GM.API.Sample.Domain.SeedWork;

public interface IUnitOfWork : IGenericUnitOfWork
{
    public ISampleRepository SampleRepository { get; }
}