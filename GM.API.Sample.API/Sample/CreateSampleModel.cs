using FluentValidation;

namespace GM.API.Sample.API.Sample;

/// <summary>
/// Create Sample
/// </summary>
public class CreateSampleModel
{
    /// <summary>
    /// The Name of the Sample
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The Description of the Sample
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The Items of the Sample
    /// </summary>
    public IEnumerable<CreateSampleItemModel>? SampleItems { get; set; }
}

/// <inheritdoc />
public class CreateSampleModelValidator : AbstractValidator<CreateSampleModel>
{
    /// <inheritdoc />
    public CreateSampleModelValidator()
    {
        RuleFor(x => x.Name).NotNull().NotEmpty();
        RuleFor(x => x.Description).NotNull().NotEmpty();
    }
}

/// <summary>
/// Create Sample Item
/// </summary>
public class CreateSampleItemModel
{
    /// <summary>
    /// The Name of the Sample Item
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The Description of the Sample Item
    /// </summary>
    public string? Description { get; set; }
}

/// <inheritdoc />
public class CreateSampleItemModelValidator : AbstractValidator<CreateSampleItemModel>
{
    /// <inheritdoc />
    public CreateSampleItemModelValidator()
    {
        RuleFor(x => x.Name).NotNull().NotEmpty();
        RuleFor(x => x.Description).NotNull().NotEmpty();
    }
}