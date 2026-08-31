using GM.API.Sample.Application.Samples.Commands.CreateSample;
using GM.API.Sample.Application.Samples.Queries.GetSampleDetails;
using GM.API.Sample.Application.Samples.Queries.GetSamplesList;
using GM.Exceptions;
using Xunit;

namespace GM.API.Sample.Tests;

public class QueryHandlerTests
{
    private static Task<int> Seed(SampleTestHost host, string name) =>
        host.ExecuteAsync(uow => new CreateSampleCommandHandler(uow).Handle(
            new CreateSampleCommand
            {
                Name = name,
                Description = name + " desc",
                SampleItems = [new CreateSampleItemCommand { Name = "i1", Description = "d1" }]
            }, CancellationToken.None));

    [Fact]
    public async Task GetDetails_returns_the_sample_with_its_items()
    {
        using var host = new SampleTestHost();
        var id = await Seed(host, "Alpha");

        var dto = await host.ExecuteAsync(uow => new GetSampleDetailsQueryHandler(uow).Handle(
            new GetSampleDetailsQuery { Id = id }, CancellationToken.None));

        Assert.Equal("Alpha", dto.Name);
        Assert.NotNull(dto.SampleItems);
        Assert.Single(dto.SampleItems);
    }

    [Fact]
    public async Task GetDetails_throws_not_found_when_missing()
    {
        using var host = new SampleTestHost();

        await Assert.ThrowsAsync<NotFoundException>(() => host.ExecuteAsync(uow =>
            new GetSampleDetailsQueryHandler(uow).Handle(
                new GetSampleDetailsQuery { Id = 123 }, CancellationToken.None)));
    }

    [Fact]
    public async Task GetList_returns_the_visible_samples()
    {
        using var host = new SampleTestHost();
        await Seed(host, "Alpha");
        await Seed(host, "Beta");

        var list = await host.ExecuteAsync(uow => new GetSamplesListQueryHandler(uow).Handle(
            new GetSamplesListQuery { CurrentPage = 1, PageSize = 10, OrderBy = "Name" }, CancellationToken.None));

        Assert.Equal(2, list.Count());
    }
}
