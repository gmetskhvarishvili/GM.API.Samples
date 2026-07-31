using GM.API.Application.Models;
using GM.API.Sample.Domain.SeedWork;
using GM.EntityFramework.Domain.Specifications;
using GM.Mediator.Contracts;
using Mapster;

namespace GM.API.Sample.Application.Samples.Queries.GetSamplesList;

public class GetSamplesListQuery : GetBaseListQuery, IRequest<IEnumerable<SampleDto>>
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}

public class GetSamplesListQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetSamplesListQuery, IEnumerable<SampleDto>>
{
    public async Task<IEnumerable<SampleDto>> Handle(GetSamplesListQuery request, CancellationToken cancellationToken)
    {
        var spec = new SampleSpecification(request.Id, request.Name, request.Description, request.CurrentPage, request.PageSize, request.OrderBy);
        var entities = await unitOfWork.SampleRepository.ListAsync(spec, cancellationToken);

        return entities.Adapt<IEnumerable<SampleDto>>();
    }
}

public class SampleDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}

public class
    SampleSpecification : BaseSpecification<Sample.Domain.BoundedContext.SampleBoundedContext.SampleAggregate.Sample>
{
    public SampleSpecification(int? id, string? name, string? description, int currentPage, int pageSize, string orderBy)
    {
        AddVisibilityFilter();

        if (id.HasValue)
            AddCriteria(s => s.Id == id.Value);

        if (!string.IsNullOrWhiteSpace(name))
            AddCriteria(s => s.Name.Contains(name));

        if (!string.IsNullOrWhiteSpace(description))
            AddCriteria(s => s.Description.Contains(description));
        
        ApplyPaging(currentPage, pageSize);
        
        ApplyOrdering(orderBy);
    }
}