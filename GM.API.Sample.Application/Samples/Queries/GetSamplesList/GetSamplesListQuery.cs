using FluentValidation;
using GM.API.Application.Models;
using GM.API.Sample.Domain.BoundedContext.SampleBoundedContext.SampleAggregate.Specifications;
using GM.API.Sample.Domain.SeedWork;
using GM.EntityFramework.Domain.Specifications;
using GM.Mediator.Contracts;
using Mapster;

namespace GM.API.Sample.Application.Samples.Queries.GetSamplesList;

/// <summary>
/// Paged sample list: by page number (<see cref="GetBaseListQuery.CurrentPage"/>), or by last key value —
/// <see cref="GetBaseListQuery.LastKeyProperty"/> (<c>Id</c> by default) + <see cref="GetBaseListQuery.LastKeyValue"/>
/// identify the last sample the client has, and the page is the samples after it in <c>OrderBy</c> order.
/// </summary>
public class GetSamplesListQuery : GetBaseListQuery, IRequest<PagedListDto<SampleDto>>
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}

/// <summary>Validates the list query, primarily its paging bounds.</summary>
public class GetSamplesListQueryValidator : AbstractValidator<GetSamplesListQuery>
{
    public GetSamplesListQueryValidator()
    {
        // The page number only matters when paging by number; a LastKeyValue pages by key instead.
        RuleFor(x => x.CurrentPage).GreaterThanOrEqualTo(1).When(x => string.IsNullOrWhiteSpace(x.LastKeyValue));
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.LastKeyProperty).MaximumLength(100);
        RuleFor(x => x.LastKeyValue).MaximumLength(250);
        RuleFor(x => x.Name).MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(250);
    }
}

public class GetSamplesListQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetSamplesListQuery, PagedListDto<SampleDto>>
{
    public async Task<PagedListDto<SampleDto>> Handle(GetSamplesListQuery request, CancellationToken cancellationToken)
    {
        var dateRange = request.ToAuditDateRange();
        var paging = request.ToPagingOptions();

        long? totalCount = null;
        if (paging.IncludeTotalCount)
        {
            var countSpec = new SampleSpecification(request.Id, request.Name, request.Description,
                dateRange, PagingOptions.None, OrderingOptions.None, request.Visibility);
            totalCount = await unitOfWork.SampleRepository.CountAsync(countSpec, cancellationToken);
        }

        var spec = new SampleSpecification(request.Id, request.Name, request.Description,
            dateRange, paging, request.ToOrderingOptions(), request.Visibility);
        var entities = await unitOfWork.SampleRepository.ListAsync(spec, cancellationToken);

        return request.ToPagedListDto(entities, entities.Adapt<List<SampleDto>>(), totalCount);
    }
}

public class SampleDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}
