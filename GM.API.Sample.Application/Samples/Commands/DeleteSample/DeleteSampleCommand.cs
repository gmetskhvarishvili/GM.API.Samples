using FluentValidation;
using GM.API.Sample.Common.Resources;
using GM.API.Sample.Domain.SeedWork;
using GM.Exceptions;
using GM.Mediator.Contracts;

namespace GM.API.Sample.Application.Samples.Commands.DeleteSample;

public class DeleteSampleCommand : IRequest
{
    public int Id { get; set; }
}

/// <summary>Validates the delete command.</summary>
public class DeleteSampleCommandValidator : AbstractValidator<DeleteSampleCommand>
{
    public DeleteSampleCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
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