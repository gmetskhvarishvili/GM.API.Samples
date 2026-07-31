using GM.API.Sample.Application.Samples.Commands.CreateSample;
using GM.API.Sample.Application.Samples.Commands.DeleteSample;
using GM.API.Sample.Application.Samples.Commands.UpdateSample;
using GM.API.Sample.Application.Samples.Queries.GetSampleDetails;
using GM.API.Sample.Application.Samples.Queries.GetSamplesList;
using Xunit;

namespace GM.API.Sample.Tests;

public class ValidatorTests
{
    [Fact]
    public void Create_rejects_empty_name_and_too_long_description()
    {
        var result = new CreateSampleCommandValidator().Validate(new CreateSampleCommand
        {
            Name = "",
            Description = new string('x', 251)
        });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateSampleCommand.Name));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateSampleCommand.Description));
    }

    [Fact]
    public void Create_validates_child_items()
    {
        var result = new CreateSampleCommandValidator().Validate(new CreateSampleCommand
        {
            Name = "ok",
            Description = "ok",
            SampleItems = [new CreateSampleItemCommand { Name = "", Description = "d" }]
        });

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Create_accepts_a_valid_command()
    {
        var result = new CreateSampleCommandValidator().Validate(new CreateSampleCommand
        {
            Name = "Widget",
            Description = "A widget"
        });

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Update_requires_a_positive_id()
    {
        var result = new UpdateSampleCommandValidator().Validate(new UpdateSampleCommand
        {
            Id = 0, Name = "n", Description = "d"
        });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateSampleCommand.Id));
    }

    [Fact]
    public void Delete_requires_a_positive_id()
    {
        Assert.False(new DeleteSampleCommandValidator().Validate(new DeleteSampleCommand { Id = 0 }).IsValid);
        Assert.True(new DeleteSampleCommandValidator().Validate(new DeleteSampleCommand { Id = 1 }).IsValid);
    }

    [Fact]
    public void GetDetails_requires_a_positive_id()
    {
        Assert.False(new GetSampleDetailsQueryValidator().Validate(new GetSampleDetailsQuery { Id = 0 }).IsValid);
        Assert.True(new GetSampleDetailsQueryValidator().Validate(new GetSampleDetailsQuery { Id = 7 }).IsValid);
    }

    [Theory]
    [InlineData(0, 10, false)]   // page must be >= 1
    [InlineData(1, 0, false)]    // size must be >= 1
    [InlineData(1, 101, false)]  // size capped at 100
    [InlineData(1, 10, true)]
    public void GetList_validates_paging_bounds(int currentPage, int pageSize, bool valid)
    {
        var result = new GetSamplesListQueryValidator().Validate(new GetSamplesListQuery
        {
            CurrentPage = currentPage, PageSize = pageSize
        });

        Assert.Equal(valid, result.IsValid);
    }
}
