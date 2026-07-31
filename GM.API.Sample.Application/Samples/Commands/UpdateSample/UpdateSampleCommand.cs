using GM.API.Sample.Common.Resources;
using GM.API.Sample.Domain.SeedWork;
using GM.Exceptions;
using GM.Mediator.Contracts;

namespace GM.API.Sample.Application.Samples.Commands.UpdateSample;

public class UpdateSampleCommand : IRequest
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// Handles the update of an existing Sample aggregate
/// </summary>
public class UpdateSampleCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateSampleCommand>
{
    public async Task Handle(UpdateSampleCommand request, CancellationToken cancellationToken)
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

        if (await unitOfWork.SampleRepository.ExistsAsync(
                x => x.Id != entity.Id
                     && x.Name == request.Name
                     && x.IsActive
                     && !x.IsDeleted
                     && !x.IsHidden,
                cancellationToken))
        {
            throw new AlreadyExistsException(
                StringResource.Sample,
                StringResource.Name,
                request.Name);
        }

        entity.Update(request.Name, request.Description);

        // Persist the aggregate
        unitOfWork.SampleRepository.Update(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}