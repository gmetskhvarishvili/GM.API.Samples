using FluentValidation;
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

/// <summary>Validates the update command; mirrors the persisted column limits.</summary>
public class UpdateSampleCommandValidator : AbstractValidator<UpdateSampleCommand>
{
    public UpdateSampleCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(250);
    }
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
                request.Name!);
        }

        // name/description are guaranteed by UpdateSampleCommandValidator
        entity.Update(request.Name!, request.Description!);

        // Persist the aggregate
        unitOfWork.SampleRepository.Update(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}