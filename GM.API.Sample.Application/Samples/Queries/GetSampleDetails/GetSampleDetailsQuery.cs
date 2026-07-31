using FluentValidation;
using GM.API.Sample.Common.Resources;
using GM.API.Sample.Domain.SeedWork;
using GM.Exceptions;
using GM.Mediator.Contracts;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace GM.API.Sample.Application.Samples.Queries.GetSampleDetails;

public class GetSampleDetailsQuery : IRequest<SampleDetailsDto>
{
    public int Id { get; set; }
}

/// <summary>Validates the details query.</summary>
public class GetSampleDetailsQueryValidator : AbstractValidator<GetSampleDetailsQuery>
{
    public GetSampleDetailsQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}

public class GetSampleDetailsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetSampleDetailsQuery, SampleDetailsDto>
{
    public async Task<SampleDetailsDto> Handle(GetSampleDetailsQuery request, CancellationToken cancellationToken)
    {
        var entity = await unitOfWork.SampleRepository
            .FirstOrDefaultAsync(x => x.Id == request.Id
                                      && x.IsActive
                                      && !x.IsDeleted
                                      && !x.IsHidden,
                false,
                x => x
                    .Include(o => o.SampleItems
                        .Where(i => i.IsActive
                                    && !i.IsDeleted
                                    && !i.IsHidden)),
                cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(
                StringResource.Sample,
                StringResource.Id,
                request.Id);
        }

        var result = entity.Adapt<SampleDetailsDto>();

        return result;
    }
}

public class SampleDetailsDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    public IEnumerable<SampleItemDto>? SampleItems { get; set; }
}

public class SampleItemDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}