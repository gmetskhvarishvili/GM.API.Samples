using GM.API.Sample.Application.Samples.Commands.CreateSample;
using GM.API.Sample.Application.Samples.Commands.DeleteSample;
using GM.API.Sample.Application.Samples.Commands.UpdateSample;
using GM.Exceptions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using SampleEntity = GM.API.Sample.Domain.BoundedContext.SampleBoundedContext.SampleAggregate.Sample;

namespace GM.API.Sample.Tests;

public class CommandHandlerTests
{
    [Fact]
    public async Task Create_persists_the_aggregate_with_items_and_stamps_audit()
    {
        using var host = new SampleTestHost();

        var id = await host.ExecuteAsync(uow => new CreateSampleCommandHandler(uow).Handle(
            new CreateSampleCommand
            {
                Name = "Widget",
                Description = "A widget",
                SampleItems =
                [
                    new CreateSampleItemCommand { Name = "part-1", Description = "first" },
                    new CreateSampleItemCommand { Name = "part-2", Description = "second" },
                ]
            }, CancellationToken.None));

        Assert.True(id > 0);

        var saved = await host.QueryAsync(ctx =>
            ctx.Set<SampleEntity>().Include(s => s.SampleItems).SingleAsync());

        Assert.Equal("Widget", saved.Name);
        Assert.Equal(2, saved.SampleItems.Count);
        Assert.NotEqual(default, saved.CreatedAt); // stamped by GenericDbContext auditing
    }

    [Fact]
    public async Task Create_throws_when_an_active_sample_with_the_same_name_exists()
    {
        using var host = new SampleTestHost();
        await host.ExecuteAsync(uow => new CreateSampleCommandHandler(uow).Handle(
            new CreateSampleCommand { Name = "Dup", Description = "x" }, CancellationToken.None));

        await Assert.ThrowsAsync<AlreadyExistsException>(() => host.ExecuteAsync(uow =>
            new CreateSampleCommandHandler(uow).Handle(
                new CreateSampleCommand { Name = "Dup", Description = "y" }, CancellationToken.None)));
    }

    [Fact]
    public async Task Update_changes_name_and_description()
    {
        using var host = new SampleTestHost();
        var id = await host.ExecuteAsync(uow => new CreateSampleCommandHandler(uow).Handle(
            new CreateSampleCommand { Name = "Old", Description = "old" }, CancellationToken.None));

        await host.ExecuteAsync(uow => new UpdateSampleCommandHandler(uow).Handle(
            new UpdateSampleCommand { Id = id, Name = "New", Description = "new" }, CancellationToken.None));

        var saved = await host.QueryAsync(ctx => ctx.Set<SampleEntity>().SingleAsync());
        Assert.Equal("New", saved.Name);
        Assert.Equal("new", saved.Description);
    }

    [Fact]
    public async Task Update_throws_not_found_when_the_sample_is_missing()
    {
        using var host = new SampleTestHost();

        await Assert.ThrowsAsync<NotFoundException>(() => host.ExecuteAsync(uow =>
            new UpdateSampleCommandHandler(uow).Handle(
                new UpdateSampleCommand { Id = 999, Name = "x", Description = "y" }, CancellationToken.None)));
    }

    [Fact]
    public async Task Delete_soft_removes_the_aggregate()
    {
        using var host = new SampleTestHost();
        var id = await host.ExecuteAsync(uow => new CreateSampleCommandHandler(uow).Handle(
            new CreateSampleCommand { Name = "Doomed", Description = "x" }, CancellationToken.None));

        await host.ExecuteAsync(uow => new DeleteSampleCommandHandler(uow).Handle(
            new DeleteSampleCommand { Id = id }, CancellationToken.None));

        var saved = await host.QueryAsync(ctx => ctx.Set<SampleEntity>().SingleAsync());
        Assert.True(saved.IsDeleted);
    }
}
