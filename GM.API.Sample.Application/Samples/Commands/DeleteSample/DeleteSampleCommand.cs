using GM.API.Sample.Common.Resources;
using GM.API.Sample.Domain.SeedWork;
using GM.Exceptions;
using GM.Mediator.Contracts;

namespace GM.API.Sample.Application.Samples.Commands.DeleteSample;

public class DeleteSampleCommand : IRequest
{
    public int Id { get; set; }
}

public class DeleteSampleCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteSampleCommand>
{
    public async Task Handle(DeleteSampleCommand request, CancellationToken cancellationToken)
    {
        // the root aggregate
        var entity = await unitOfWork.SampleRepository
            .FirstOrDefaultAsync(x => x.Id == request.Id
                                      && x.IsActive
                                      && !x.IsDeleted
                                      && !x.IsHidden,
                true,
                null,
                cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(
                StringResource.Sample,
                StringResource.Id,
                request.Id);
        }

        entity.SoftRemove();

        // Persist the aggregate
        unitOfWork.SampleRepository.Update(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}