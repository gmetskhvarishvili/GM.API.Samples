using GM.API.Application.Models;
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

        Assert.Equal(2, list.Items.Count);
        Assert.Equal(2, list.TotalCount);
    }

    private static async Task<SampleTestHost> HostWithFiveSamplesAsync()
    {
        var host = new SampleTestHost();
        foreach (var name in new[] { "A", "B", "C", "D", "E" }) // Ids 1..5
            await Seed(host, name);
        return host;
    }

    private static Task<PagedListDto<SampleDto>> ListAsync(SampleTestHost host, GetSamplesListQuery query) =>
        host.ExecuteAsync(uow => new GetSamplesListQueryHandler(uow).Handle(query, CancellationToken.None));

    [Fact]
    public async Task GetList_by_page_number_reports_totals_and_the_last_key()
    {
        using var host = await HostWithFiveSamplesAsync();

        var page = await ListAsync(host, new GetSamplesListQuery { CurrentPage = 2, PageSize = 2, OrderBy = "Id asc" });

        Assert.Equal(["C", "D"], page.Items.Select(s => s.Name));
        Assert.Equal(2, page.CurrentPage);
        Assert.Equal(5, page.TotalCount);
        Assert.Equal(3, page.TotalPages);
        Assert.True(page.HasMore);
        Assert.Equal("Id", page.LastKeyProperty);
        Assert.Equal("4", page.LastKeyValue);
    }

    [Fact]
    public async Task GetList_by_last_key_value_walks_the_samples_after_it()
    {
        using var host = await HostWithFiveSamplesAsync();

        var first = await ListAsync(host, new GetSamplesListQuery { CurrentPage = 1, PageSize = 2, OrderBy = "Id asc" });
        var second = await ListAsync(host, new GetSamplesListQuery
        {
            PageSize = 2, OrderBy = "Id asc", LastKeyProperty = first.LastKeyProperty, LastKeyValue = first.LastKeyValue
        });

        Assert.Equal(["A", "B"], first.Items.Select(s => s.Name));
        Assert.Equal(["C", "D"], second.Items.Select(s => s.Name));
        Assert.Equal(0, second.CurrentPage); // paged by key, not by number
        Assert.True(second.HasMore);
    }

    [Fact]
    public async Task GetList_by_a_named_key_continues_in_the_requested_order()
    {
        using var host = await HostWithFiveSamplesAsync();

        // "B" identifies the last sample the client has (Id 2); ordered by Id descending, next is Id 1.
        var page = await ListAsync(host, new GetSamplesListQuery
        {
            PageSize = 10, LastKeyProperty = "Name", LastKeyValue = "B", OrderBy = "Id desc"
        });

        Assert.Equal(["A"], page.Items.Select(s => s.Name));
        Assert.Equal("Name", page.LastKeyProperty);
        Assert.Equal("A", page.LastKeyValue);
        Assert.False(page.HasMore);
    }

    [Fact]
    public async Task GetList_skips_the_total_count_when_not_wanted()
    {
        using var host = await HostWithFiveSamplesAsync();

        var page = await ListAsync(host, new GetSamplesListQuery
        {
            CurrentPage = 1, PageSize = 2, OrderBy = "Id asc", IncludeTotalCount = false
        });

        Assert.False(page.HasTotalCount);
        Assert.Equal(0, page.TotalCount);
        Assert.True(page.HasMore); // without a total, a full page means there may be more
    }
}
