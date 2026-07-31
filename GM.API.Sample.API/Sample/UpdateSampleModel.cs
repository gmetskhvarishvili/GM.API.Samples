using FluentValidation;

namespace GM.API.Sample.API.Sample;

/// <summary>
/// Update Sample
/// </summary>
public class UpdateSampleModel
{
    /// <summary>
    /// The Name of the Sample
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// The Description of the Sample
    /// </summary>
    public string? Description { get; set; }
}

/// <inheritdoc />
public class UpdateSampleModelValidator : AbstractValidator<UpdateSampleModel>
{
    /// <inheritdoc />
    public UpdateSampleModelValidator()
    {
        RuleFor(x => x.Name).NotNull().NotEmpty();
        RuleFor(x => x.Description).NotNull().NotEmpty();
    }
}