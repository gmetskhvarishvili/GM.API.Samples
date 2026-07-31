using FluentValidation;
using GM.API.Sample.Common.Resources;
using GM.API.Sample.Domain.BoundedContext.SampleBoundedContext.SampleAggregate;
using GM.API.Sample.Domain.SeedWork;
using GM.Exceptions;
using GM.Mediator.Contracts;

namespace GM.API.Sample.Application.Samples.Commands.CreateSample;

public class CreateSampleCommand : IRequest<int>
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    public IEnumerable<CreateSampleItemCommand>? SampleItems { get; set; }
}

/// <summary>Validates the create command; mirrors the persisted column limits.</summary>
public class CreateSampleCommandValidator : AbstractValidator<CreateSampleCommand>
{
    public CreateSampleCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(250);
        RuleForEach(x => x.SampleItems).SetValidator(new CreateSampleItemCommandValidator());
    }
}

public class CreateSampleItemCommand
{
    public string? Name { get; set; }
    public string? Description { get; set; }
}

/// <summary>Validates each child item on the create command.</summary>
public class CreateSampleItemCommandValidator : AbstractValidator<CreateSampleItemCommand>
{
    public CreateSampleItemCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(250);
    }
}

/// <summary>
/// Handles the creation of a new Sample aggregate with optional sample items.
/// </summary>
public class CreateSampleCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateSampleCommand, int>
{
    public async Task<int> Handle(CreateSampleCommand request, CancellationToken cancellationToken)
    {
        if (await unitOfWork.SampleRepository.ExistsAsync(
                x => x.Name == request.Name
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

        // Create the root aggregate (name/description are guaranteed by CreateSampleCommandValidator)
        var entity = Domain.BoundedContext.SampleBoundedContext.SampleAggregate.Sample
            .Create(request.Name!, request.Description!);

        // Add child items if any
        if (request.SampleItems?.Any() == true)
        {
            var items = request.SampleItems
                .Select(item => SampleItem.Create(item.Name!, item.Description!))
                .ToArray();
            
            entity.AddSampleItems(items);
        }

        // Persist the aggregate
        await unitOfWork.SampleRepository.AddAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
